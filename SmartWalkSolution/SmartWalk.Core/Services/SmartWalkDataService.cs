using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml;
using System.Xml.Linq;
using MonoTouch.Foundation;
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

        public void GetLocation(
            string name,
            DataSource source,
            Action<Location, Exception> resultHandler)
        {
            GetGenericContract<Location>(
                name,
                TempURL + "index.xml",
                CreateLocation,
                source,
                resultHandler);
        }

        public void GetOrg(
            string orgId,
            DataSource source,
            Action<Org, Exception> resultHandler)
        {
            GetGenericContract<Org>(
                orgId,
                TempURL + orgId + "/index.xml",
                new Func<XDocument, Org>(doc => CreateOrg(orgId, doc)),
                source,
                resultHandler);
        }

        public void GetOrgEvent(
            string orgId,
            DateTime date,
            DataSource source,
            Action<OrgEvent, Exception> resultHandler)
        {
            var key = orgId + "-" + String.Format("{0:yyyy-MM-dd}", date);

            GetGenericContract<OrgEvent>(
                key,
                TempURL + orgId + "/events/" + key + ".xml",
                new Func<XDocument, OrgEvent>(doc => CreateOrgEvent(orgId, date, doc)),
                source,
                resultHandler);
        }

        private void GetGenericContract<T>(
            string key,
            string url,
            Func<XDocument, T> createContract,
            DataSource source, Action<T, Exception> resultHandler)
                where T : class
        {
            var xml = default(XDocument);
            var isConnected = GetIsConnected();
            var processXmlHandler = new Action<XDocument>(doc => {
                if (doc != null)
                {
                    var contract = createContract(doc);

                    _cacheService.SetString(key, doc.ToString());

                    resultHandler(contract, null);
                }
                else
                {
                    resultHandler(null, null);
                }
            });

            if (!isConnected || source == DataSource.Cache)
            {
                var data = _cacheService.GetString(key);
                if (data != null)
                {
                    try
                    {
                        xml = XDocument.Parse(data);
                    }
                    catch (Exception xmlEx)
                    {
                        resultHandler(null, xmlEx);
                        return;
                    }
                }
            }

            if (isConnected && xml == null)
            {
                DownloadString(
                    url,
                    (result, ex) =>
                    {
                    if (ex == null && result != null)
                        {
                            XDocument xmlDoc;

                            try
                            {
                                xmlDoc = XDocument.Load(
                                    XmlReader.Create(
                                        new MemoryStream(result)));
                            }
                            catch (Exception xmlEx)
                            {
                                resultHandler(null, xmlEx);
                                return;
                            }

                            processXmlHandler(xmlDoc);
                        }
                        else
                        {
                            resultHandler(null, ex);
                        }
                    });
            }
            else
            {
                processXmlHandler(xml);
            }
        }

        private static bool GetIsConnected()
        {
            return Reachability.IsHostReachable(host);
        }

        private static void DownloadString(string url, Action<byte[], Exception> resultHandler)
        {
            var client = new WebClient();
            client.DownloadDataCompleted += (s, e) => 
                HoldOnABit(() => 
                    InvokeOnMainThread(() => 
                        resultHandler(e.Result, e.Error)));
            client.DownloadDataAsync(new Uri(url));
        }

        /// <summary>
        /// A debug only trick to simulate slower network connection,
        /// for testing loading progress indication.
        /// </summary>
        private static void HoldOnABit(Action handler)
        {
#if DEBUG
            NSThread.SleepFor(0.5);
            handler();
#else
            handler();
#endif
        }

        private static void InvokeOnMainThread(Action handler)
        {
            using (var obj = new NSObject())
            {
                obj.InvokeOnMainThread(new NSAction(handler));
            }
        }

        private static Location CreateLocation(XDocument xml)
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
            return result;
        }

        private static Org CreateOrg(string orgId, XDocument xml)
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

            return result;
        }

        private static OrgEvent CreateOrgEvent(string orgId, DateTime date, XContainer xml)
        {
            var result = new OrgEvent {
                Info = new OrgEventInfo
                    {
                        OrgId = orgId,
                        Date = date
                    },
                Venues = xml.Descendants("venue")
                        .Select(v => 
                        {
                            int number;
                            int.TryParse(v.Attribute("number") != null 
                                     ? v.Attribute("number").Value 
                                     : "0", out number);

                            var venue = new Venue 
                                {
                                    Number = number,
                                    Info = new EntityInfo 
                                    {
                                        Name = v.Attribute("name").ValueOrNull(),
                                        Logo = v.Attribute("logo").ValueOrNull(),
                                        Addresses = v.Descendants("point")
                                            .Select(point => CreateAddress(
                                                point.Attribute("coordinates").ValueOrNull(),
                                                point.ValueOrNull()))
                                            .ToArray(),
                                        Contact = CreateContact(v)
                                    },
                                    Description = v.Element("description").ValueOrNull(),
                                    Shows = v.Descendants("show")
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
                            
                            return venue;
                        }).ToArray()
            };
            return result;
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

            var result = new AddressInfo 
                {
                    Latitude = latitude,
                    Longitude = longitude,
                    Address = address,
                };
            return result;
        }

        private static ContactInfo CreateContact(XContainer entity)
        {
            var result = new ContactInfo
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
            return result;
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
                    0);

                // all night AM time should be set to the next day
                if (parsedStartTime > parsedEndTime)
                {
                    parsedEndTime = parsedEndTime.AddDays(1);
                }
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