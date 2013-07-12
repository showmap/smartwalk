using System;
using System.Linq;
using System.Xml.Linq;
using SmartWalk.Core.Model;

namespace SmartWalk.Core.Services
{
    public class SmartWalkDataService : ISmartWalkDataService
    {
        private readonly IExceptionPolicy _exceptionPolicy;

        public SmartWalkDataService(IExceptionPolicy exceptionPolicy)
        {
            _exceptionPolicy = exceptionPolicy;
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
                            Logo = "TempXML/Local/" + org.Attribute("id").ValueOrNull() + 
                                "/" + org.Attribute("logo").ValueOrNull(),
                        }).ToArray();

                resultHandler(result, null);
            }
            catch (Exception ex)
            {
                _exceptionPolicy.Trace(ex);
                resultHandler(null, ex);
            }
        }

        public void GetOrg(string orgId, Action<Org, Exception> resultHandler)
        {
            try
            {
                var xml = XDocument.Load(@"TempXML/Local/" + orgId + "/index.xml");
                var result = new Org {
                    Info = new EntityInfo{
                        Id = orgId,
                        Name = xml.Root.Attribute("name").ValueOrNull(),
                        Logo = "TempXML/Local/" + orgId + "/" + xml.Root.Attribute("logo").ValueOrNull(),
                    },
                    Description = xml.Root.Element("description").ValueOrNull(),
                };

                if (orgId == "nbff")
                {
                    result.EventInfos = new [] { 
                        new OrgEventInfo 
                        { 
                            OrgId = "nbff",
                            Date = new DateTime(2013, 6, 7)
                        },
                        new OrgEventInfo 
                        { 
                            OrgId = "nbff",
                            Date = new DateTime(2013, 7, 5)
                        }
                    };
                }
                else if (orgId == "mapp")
                {
                    result.EventInfos = new [] { 
                        new OrgEventInfo 
                        { 
                            OrgId = "mapp",
                            Date = new DateTime(2013, 4, 1)
                        },
                        new OrgEventInfo 
                        { 
                            OrgId = "mapp",
                            Date = new DateTime(2013, 6, 1)
                        },
                        new OrgEventInfo 
                        { 
                            OrgId = "mapp",
                            Date = new DateTime(2013, 8, 3)
                        },
                        new OrgEventInfo 
                        { 
                            OrgId = "mapp",
                            Date = new DateTime(2013, 10, 3)
                        }
                    };
                }

                resultHandler(result, null);
            }
            catch (Exception ex)
            {
                _exceptionPolicy.Trace(ex);
                resultHandler(null, ex);
            }
        }

        public void GetOrgEvent(string orgId, DateTime date, Action<OrgEvent, Exception> resultHandler)
        {
            try
            {
                var xml = XDocument.Load(@"TempXML/Local/" + orgId + "/" + 
                    orgId + "-" + String.Format("{0:yyyy-MM-dd}", date) + ".xml");

                var result = new OrgEvent
                    {
                        Info = new OrgEventInfo {
                            OrgId = orgId,
                            Date = date
                        },
                        Venues = xml.Descendants("venue").Select(venue => 
                            new Venue 
                            {
                                Number = venue.Attribute("number") != null 
                                    ? int.Parse(venue.Attribute("number").Value) : 0,
                                Info = new EntityInfo {
                                    Name = venue.Attribute("name").ValueOrNull()
                                },
                                Description = venue.Element("description").ValueOrNull(),
                                Shows = venue.Descendants("show").Select(show => 
                                    new VenueShow
                                    {
                                        Start = show.Attribute("start") != null 
                                            ? DateTime.Parse(show.Attribute("start").Value) : default(DateTime),
                                        End = show.Attribute("end") != null 
                                            ? DateTime.Parse(show.Attribute("end").Value) : default(DateTime),
                                        Description = show.Value,
                                    }).ToArray()
                            }).ToArray()
                    };

                resultHandler(result, null);
            }
            catch (Exception ex)
            {
                _exceptionPolicy.Trace(ex);
                resultHandler(null, ex);
            }
        }
    }

    public static class XMLExtensions
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