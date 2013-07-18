using System;
using System.Linq;
using System.Xml.Linq;
using SmartWalk.Core.Model;

namespace SmartWalk.Core.Services
{
    public class SmartWalkDataService : ISmartWalkDataService
    {
        private readonly ICacheService _cacheService;

        public SmartWalkDataService(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }

        public void GetOrgInfos(Action<EntityInfo[], Exception> resultHandler)
        {
            try
            {
                var xml = XDocument.Load(@"TempXML/Local/index.xml");
                var result = xml.Descendants("organization").Select(org => 
                    new EntityInfo 
                        {
                            Id = org.Attribute("id").ValueOrNull(),
                            Name = org.Attribute("name").ValueOrNull(),
                            Logo = org.Attribute("logo").ValueOrNull(),
                        }).ToArray();

                resultHandler(result, null);
            }
            catch (Exception ex)
            {
                resultHandler(null, ex);
            }
        }

        public void GetOrg(string orgId, Action<Org, Exception> resultHandler)
        {
            try
            {
                var xml = XDocument.Load(@"TempXML/Local/" + orgId + "/index.xml");
                var result = new Org
                {
                    Info = new EntityInfo 
                        {
                            Id = orgId,
                            Name = xml.Root != null ? xml.Root.Attribute("name").ValueOrNull() : null,
                            Logo = xml.Root != null ? xml.Root.Attribute("logo").ValueOrNull() : null,
                        },
                    Description = xml.Root != null ? xml.Root.Element("description").ValueOrNull() : null,
                };

                result.EventInfos = xml.Descendants("event")
                    .Select(org => 
                        new OrgEventInfo 
                        {
                            OrgId = orgId,
                            Date = DateTime.Parse(org.Attribute("date").ValueOrNull()),
                            HasSchedule = org.Attribute("hasSchedule").ValueOrNull() == "true"
                        }).ToArray();

                resultHandler(result, null);
            }
            catch (Exception ex)
            {
                resultHandler(null, ex);
            }
        }

        public void GetOrgEvent(string orgId, DateTime date, DataSource source, Action<OrgEvent, Exception> resultHandler)
        {
            try
            {
                var key = orgId + "-" + String.Format("{0:yyyy-MM-dd}", date);
                var xml = default(XDocument);

                if (source == DataSource.Cache)
                {
                    var data = _cacheService.GetString(key);
                    if (data != null)
                    {
                        xml = XDocument.Parse(data);
                    }
                }

                if (xml == null)
                {
                    xml = XDocument.Load(@"TempXML/Local/" + orgId + "/" + key + ".xml");
                }

                if (xml != null)
                {
                    var result = CreateOrgEvent(orgId, date, xml);

                    _cacheService.SetString(key, xml.ToString());

                    resultHandler(result, null);
                }
            }
            catch (Exception ex)
            {
                resultHandler(null, ex);
            }
        }

        private OrgEvent CreateOrgEvent(string orgId, DateTime date, XDocument xml)
        {
            return new OrgEvent 
                {
                    Info = new OrgEventInfo
                            {
                                OrgId = orgId,
                                Date = date
                            },
                    Venues = xml.Descendants("venue")
                        .Select(venue => new Venue 
                            {
                                Number = venue.Attribute("number") != null 
                                    ? int.Parse(venue.Attribute("number").Value) : 0,
                                    Info = new EntityInfo 
                                {
                                    Name = venue.Attribute("name").ValueOrNull(),
                                    Logo = venue.Attribute("logo").ValueOrNull(),
                                    Addresses = venue.Descendants("point")
                                        .Select(point => CreateAddress(
                                            point.Attribute("coordinates").ValueOrNull(),
                                            point.ValueOrNull()))
                                        .ToArray()
                                },
                                Description = venue.Element("description").ValueOrNull(),
                                Shows = venue.Descendants("show")
                                    .Select(show => new VenueShow
                                        {
                                            Start = show.Attribute("start") != null 
                                                ? DateTime.Parse(show.Attribute("start").Value) : default(DateTime),
                                                End = show.Attribute("end") != null 
                                                    ? DateTime.Parse(show.Attribute("end").Value) : default(DateTime),
                                                    Description = show.Value,
                                        }).ToArray()
                            }).ToArray()
                };
        }

        private AddressInfo CreateAddress(string coordinates, string address)
        {
            var latitude = default(double);
            var longitude = default(double);

            if (coordinates != null)
            {
                var points = coordinates.Split(',');

                if (points.Length == 2)
                {
                    double.TryParse(points[0], out latitude);
                    double.TryParse(points[1], out longitude);
                }
            }

            return new AddressInfo 
                {
                    Latitude = latitude,
                    Longitude = longitude,
                    Address = address,
                };
        }
    }

    public static class XExtensions
    {
        public static string ValueOrNull(this XAttribute attribute)
        {
            return attribute != null ? attribute.Value : null;
        }

        public static string ValueOrNull(this XElement element)
        {
            return element != null ? element.Value : null;
        }
    }
}