using System;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using SmartWalk.XmlData.Model;

namespace SmartWalk.XmlData
{
    class Program
    {
        private const string XmlDataPath = "../../XmlData";

        static void Main(string[] args)
        {
            var location = ParseLocation("sfbay");

            // TODO: to store it in SmartWalk Orchard Storage
        }

        private static Location ParseLocation(string regionName)
        {
            using (var indexXmlReader = 
                XmlReader.Create(string.Format("{0}/{1}/index.xml", XmlDataPath, regionName)))
            {
                var serializer = new XmlSerializer(typeof(Location));
                var location = (Location)serializer.Deserialize(indexXmlReader);

                location.Organizations = location.Organizations
                    .Select(org => ParseOrganization(regionName, org.Id))
                    .ToArray();

                return location;
            }
        }

        private static Organization ParseOrganization(string regionName, string orgId)
        {
            using (var indexXmlReader = 
                XmlReader.Create(string.Format("{0}/{1}/{2}/index.xml", XmlDataPath, regionName, orgId)))
            {
                var serializer = new XmlSerializer(typeof(Organization));
                var organization = (Organization)serializer.Deserialize(indexXmlReader);

                organization.Events = organization.EventRefs
                    .Where(er => er.HasSchedule)
                    .Select(er => ParseEvent(regionName, orgId, er.DateObject))
                    .ToArray();

                return organization;
            }
        }

        private static Event ParseEvent(string regionName, string orgId, DateTime eventDate)
        {
            using (var indexXmlReader =
                XmlReader.Create(string.Format(
                    "{0}/{1}/{2}/events/{2}-{3}.xml", 
                    XmlDataPath, 
                    regionName, 
                    orgId,
                    String.Format("{0:yyyy-MM-dd}", eventDate))))
            {
                var serializer = new XmlSerializer(typeof(Event));
                var eventObj = (Event)serializer.Deserialize(indexXmlReader);

                foreach (var show in eventObj.Venues
                    .Where(v => v.Shows != null)
                    .SelectMany(v => v.Shows)
                    .ToArray())
                {
                    show.ParseShowTime(eventDate);
                }

                return eventObj;
            }
        }
    }
}