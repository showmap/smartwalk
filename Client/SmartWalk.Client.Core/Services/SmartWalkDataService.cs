using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using RestSharp.Portable;
using SmartWalk.Client.Core.Model;

namespace SmartWalk.Client.Core.Services
{
    public class SmartWalkDataService : ISmartWalkDataService
    {
        private const string host = "smartwalk.me";
        private const string dataUrl = "http://" + host + "/data/";

        private readonly ICacheService _cacheService;
        private readonly ILocationService _locationService;
        private readonly IReachabilityService _reachabilityService;

        public SmartWalkDataService(
            ICacheService cacheService,
            ILocationService locationService,
            IReachabilityService reachabilityService)
        {
            _cacheService = cacheService;
            _locationService = locationService;
            _reachabilityService = reachabilityService;
        }

        public async Task<LocationIndex> GetLocationIndex(DataSource source)
        {
            var location = _locationService.CurrentLocation;
            var key = GenerateKey(location);
            var result = await GetGenericContract<LocationIndex>(
                key,
                dataUrl + location + "index.xml",
                CreateLocation,
                source);
            return result;
        }

        public async Task<Org> GetOrg(string orgId, DataSource source)
        {
            var location = _locationService.CurrentLocation;
            var key = GenerateKey(location, orgId);
            var result = await GetGenericContract<Org>(
                key,
                dataUrl + location + orgId + "/index.xml",
                new Func<XDocument, Org>(doc => CreateOrg(orgId, doc)),
                source);
            return result;
        }

        public async Task<OrgEvent> GetOrgEvent(string orgId, DateTime date, DataSource source)
        {
            var location = _locationService.CurrentLocation;
            var key = GenerateKey(
                location,
                orgId,
                String.Format("{0:yyyy-MM-dd}", date));
            var result = await GetGenericContract<OrgEvent>(
                key,
                dataUrl + location + orgId + "/events/" + 
                    orgId + "-" + String.Format("{0:yyyy-MM-dd}", date) + ".xml",
                new Func<XDocument, OrgEvent>(doc => CreateOrgEvent(orgId, date, doc)),
                source);
            return result;
        }

        private async Task<T> GetGenericContract<T>(
            string key,
            string url,
            Func<XDocument, T> createContract,
            DataSource source)
                where T : class
        {
            var xml = default(XDocument);
            var isConnected = GetIsConnected();
            var processXmlHandler = new Func<XDocument, T>(doc =>
            {
                if (doc != null)
                {
                    var contract = createContract(doc);

                    _cacheService.SetString(key, doc.ToString());

                    return contract;
                }

                return null;
            });

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
                var result = await DownloadString(url);
                if (result != null)
                {
                    var xmlDoc = 
                        XDocument.Load(
                            XmlReader.Create(
                                new MemoryStream(result)));

                    return processXmlHandler(xmlDoc);
                }
            }
            else
            {
                return processXmlHandler(xml);
            }

            return null;
        }

        private bool GetIsConnected()
        {
            return _reachabilityService.IsHostReachable(host);
        }

        private async Task<byte[]> DownloadString(string url)
        {
            var client = new RestClient(new Uri(url));
            var request = new RestRequest(string.Empty);
            var result = await client.Execute(request);
            return result != null ? result.RawBytes : null;
        }

        private static string GenerateKey(params string[] args)
        {
            var key = default(string);
            var comp = StringComparison.OrdinalIgnoreCase;

            if (args != null)
            {
                if (args.Length == 1)
                {
                    key = args[0].Replace("/", "-").TrimEnd('-');
                }
                else if (args.Length > 0)
                {
                    key = args.Aggregate((a, b) => 
                        {
                            var one = a.Replace("/", "-");
                            var another = b.Replace("/", "-");

                            return one + 
                                (one.EndsWith("-", comp) ? string.Empty : "-") + 
                                another;
                        });
                }
            }

            return key;
        }

        private static LocationIndex CreateLocation(XDocument xml)
        {
            var result = new LocationIndex
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
                        Date = DateTime.Parse(org.Attribute("date").ValueOrNull(), CultureInfo.InvariantCulture),
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
                    double.TryParse(
                        points[0],
                        NumberStyles.Float,
                        CultureInfo.InvariantCulture,
                        out latitude);
                    double.TryParse(
                        points[1],
                        NumberStyles.Float,
                        CultureInfo.InvariantCulture,
                        out longitude);
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

        private static Tuple<DateTime, DateTime> ParseShowTime(
            string start,
            string end,
            DateTime eventDate)
        {
            DateTime parsedStartTime;
            if (start != null && DateTime.TryParse(
                    start, 
                    CultureInfo.InvariantCulture, 
                    DateTimeStyles.None, 
                    out parsedStartTime))
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
            if (end != null && DateTime.TryParse(
                    end, 
                    CultureInfo.InvariantCulture, 
                    DateTimeStyles.None, 
                    out parsedEndTime))
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