using System;
using System.Collections.Generic;
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

        public void GetOrgInfos(Action<IEnumerable<OrgInfo>, Exception> resultHandler)
        {
            try
            {
                var xml = XDocument.Load(@"TempXML/Local/index.xml");
                var result = xml.Descendants("organization").Select(org => 
                    new OrgInfo 
                        {
                            Id = org.Attribute("id").Value,
                            Name = org.Attribute("name").Value,
                            Logo = "TempXML/Local/" + org.Attribute("id").Value + 
                                "/" + org.Attribute("logo").Value,
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
                    Info = new OrgInfo{
                        Id = orgId,
                        Name = xml.Root.Attribute("name").Value,
                        Logo = "TempXML/Local/" + orgId + "/" + xml.Root.Attribute("logo").Value,
                    },
                    Description = xml.Root.Element("description").Value,
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

        public void GetOrgEvent(OrgEventInfo eventInfo, Action<OrgEvent, Exception> resultHandler)
        {
            try
            {
                var xml = XDocument.Load(@"TempXML/Local/" + eventInfo.OrgId + "/" + 
                     eventInfo.OrgId + "-" + String.Format("{0:yyyy-mm-dd}", eventInfo.Date));

                var result = new OrgEvent
                    {
                        Info = eventInfo,
                        Venues = xml.Descendants("venue").Select(venue => 
                            new Venue 
                            {
                                Number = int.Parse(venue.Attribute("number").Value),
                                Name = venue.Attribute("name").Value,
                                Description = venue.Attribute("description").Value,
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
}