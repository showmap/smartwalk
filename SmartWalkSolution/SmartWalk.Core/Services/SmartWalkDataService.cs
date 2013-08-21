using System;
using System.Linq;
using System.Xml.Linq;
using SmartWalk.Core.Model;
using SmartWalk.Core.Utils;

namespace SmartWalk.Core.Services
{
    public class SmartWalkDataService : ISmartWalkDataService
    {
        private const string host = "smartwalk.me";
        private const string TempURL = "http://" + host + "/data/us/ca/sfbay/";

        private readonly ICacheService _cacheService;

        public SmartWalkDataService(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }

        public void GetLocation(string name, DataSource source, Action<Location, Exception> resultHandler)
        {
            try
            {
                var key = name;
                var xml = default(XDocument);
                var isConnected = GetIsConnected();

                if (!isConnected || source == DataSource.Cache)
                {
                    var data = _cacheService.GetString(key);
                    if (data != null)
                    {
                        xml = XDocument.Parse(data);
                    }
                }

                if (isConnected && xml == null)
                {
                    xml = XDocument.Load(TempURL + "index.xml");
                }

                if (xml != null)
                {
                    var result = new Location
                        {
                            Name = xml.Root.Attribute("name").ValueOrNull(),
                            Logo = xml.Root.Attribute("logo").ValueOrNull(),
                            OrgInfos = xml.Descendants("organization").Select(
                                org => 
                                    new EntityInfo 
                                    {
                                        Id = org.Attribute("id").ValueOrNull(),
                                        Name = org.Attribute("name").ValueOrNull(),
                                        Logo = org.Attribute("logo").ValueOrNull()
                                    }).ToArray()
                        };

                    _cacheService.SetString(key, xml.ToString());

                    resultHandler(result, null);
                }
                else
                {
                    resultHandler(null, null);
                }
            }
            catch (Exception ex)
            {
                resultHandler(null, ex);
            }
        }

        public void GetOrg(string orgId, DataSource source, Action<Org, Exception> resultHandler)
        {
            try
            {
                var key = orgId;
                var xml = default(XDocument);
                var isConnected = GetIsConnected();

                if (!isConnected || source == DataSource.Cache)
                {
                    var data = _cacheService.GetString(key);
                    if (data != null)
                    {
                        xml = XDocument.Parse(data);
                    }
                }

                if (isConnected && xml == null)
                {
                    xml = XDocument.Load(TempURL + orgId + "/index.xml");
                }

                if (xml != null)
                {
                    var result = new Org
                        {
                            Info = new EntityInfo 
                                {
                                    Id = orgId,
                                    Name = xml.Root != null ? xml.Root.Attribute("name").ValueOrNull() : null,
                                    Logo = xml.Root != null ? xml.Root.Attribute("logo").ValueOrNull() : null,
                                    Contact = CreateContact(xml)
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

                    _cacheService.SetString(key, xml.ToString());

                    resultHandler(result, null);
                }
                else
                {
                    resultHandler(null, null);
                }
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
                var isConnected = GetIsConnected();

                if (!isConnected || source == DataSource.Cache)
                {
                    var data = _cacheService.GetString(key);
                    if (data != null)
                    {
                        xml = XDocument.Parse(data);
                    }
                }

                if (isConnected && xml == null)
                {
                    xml = XDocument.Load(TempURL + orgId + "/events/" + key + ".xml");
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

        private static bool GetIsConnected()
        {
            return Reachability.IsHostReachable(host);
        }

        private static OrgEvent CreateOrgEvent(string orgId, DateTime date, XContainer xml)
        {
            return new OrgEvent {
                Info = new OrgEventInfo
                    {
                        OrgId = orgId,
                        Date = date
                    },
                Venues = xml.Descendants("venue")
                        .Select(venue => 
                        {
                            int number;
                            int.TryParse(venue.Attribute("number") != null 
                                     ? venue.Attribute("number").Value 
                                     : "0", out number);

                            var result = new Venue 
                                {
                                    Number = number,
                                    Info = new EntityInfo 
                                    {
                                        Name = venue.Attribute("name").ValueOrNull(),
                                        Logo = venue.Attribute("logo").ValueOrNull(),
                                        Addresses = venue.Descendants("point")
                                            .Select(point => CreateAddress(
                                                point.Attribute("coordinates").ValueOrNull(),
                                                point.ValueOrNull()))
                                            .ToArray(),
                                        Contact = CreateContact(venue)
                                    },
                                    Description = venue.Element("description").ValueOrNull(),
                                    Shows = venue.Descendants("show")
                                        .Select(show => {
                                            var times = ParseShowTime(show.Attribute("start").ValueOrNull(),
                                                show.Attribute("end").ValueOrNull(), date);
                                            return new VenueShow
                                                {
                                                    Start = times.Item1,
                                                    End = times.Item2,
                                                    Description = show.Value,
                                                    Logo = show.Attribute("logo").ValueOrNull(),
                                                    Site = show.Attribute("web") != null 
                                                        ? new WebSiteInfo
                                                        {
                                                            URL = show.Attribute("web").ValueOrNull()
                                                        }
                                                        : null
                                                };
                                            }).ToArray()
                                };
                            
                            return result;
                        }).ToArray()
            };
        }

        private static AddressInfo CreateAddress(string coordinates, string address)
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

        private static ContactInfo CreateContact(XContainer entity)
        {
            return new ContactInfo
                {
                    Phones = entity.Descendants("phone")
                        .Select(phone => new PhoneInfo 
                            {
                                Name = phone.Attribute("name").ValueOrNull(),
                                Phone = phone.ValueOrNull(),
                        }).ToArray(),
                    Emails = entity.Descendants("email")
                        .Select(email => new EmailInfo 
                            {
                                Name = email.Attribute("name").ValueOrNull(),
                                Email = email.ValueOrNull(),
                            }).ToArray(),
                    WebSites = entity.Descendants("web")
                        .Select(web => new WebSiteInfo
                            {
                                URL = web.ValueOrNull()
                            }).ToArray(),
                };
        }

        private static Tuple<DateTime, DateTime> ParseShowTime(string start, string end, DateTime eventDate)
        {
            DateTime parsedStartTime;
            if (start != null && DateTime.TryParse(start, out parsedStartTime))
            {
                parsedStartTime = new DateTime(
                    eventDate.Year, 
                    eventDate.Month, 
                    eventDate.Day, 
                    parsedStartTime.Hour, 
                    parsedStartTime.Minute,
                    0);
            }
            else
            {
                parsedStartTime = DateTime.MinValue;
            }

            DateTime parsedEndTime;
            if (end != null && DateTime.TryParse(end, out parsedEndTime))
            {
                parsedEndTime = new DateTime(
                    eventDate.Year, 
                    eventDate.Month, 
                    eventDate.Day, 
                    parsedEndTime.Hour, 
                    parsedEndTime.Minute,
                    0).AddDays(parsedStartTime > parsedEndTime ? 1 : 0);
            }
            else
            {
                parsedEndTime = DateTime.MaxValue;
            }

            return new Tuple<DateTime, DateTime>(parsedStartTime, parsedEndTime);
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