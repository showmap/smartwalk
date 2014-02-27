using System;
using System.Net;
using Newtonsoft.Json;
using SmartWalk.Shared.DataContracts;
using SmartWalk.Shared.DataContracts.Api;
using System.Linq;

namespace SmartWalk.Labs.Api
{
    public class MainClass
    {
        private const string SmartWalkUrl = @"http://smartwalk.com:8091/api";

        public static void Main(string[] args)
        {
            Console.WriteLine("SmartWalk Protocol JSON objects demo");

            Console.WriteLine("\nHome View");

            var homeViewRequest = CreateHomeViewRequest("United States", "California", "San Francisco");
            var json = JsonConvert.SerializeObject(
                homeViewRequest,
                new JsonSerializerSettings
                    {
                        Formatting = Formatting.Indented,
                        NullValueHandling = NullValueHandling.Ignore
                    });
            Console.WriteLine(json);

            using (var client = new WebClient())
            {
                client.Headers[HttpRequestHeader.ContentType] = "application/json";

                try
                {
                    var result = client.UploadString(SmartWalkUrl, json);
                    Console.WriteLine(result);

                    var response = JsonConvert.DeserializeObject<Response>(result);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            Console.WriteLine("\nEvent View");
            var eventViewRequest = CreateEventViewRequest(5);
            json = JsonConvert.SerializeObject(
                eventViewRequest,
                new JsonSerializerSettings
                    {
                        Formatting = Formatting.Indented,
                        NullValueHandling = NullValueHandling.Ignore
                    });
            Console.WriteLine(json);

            using (var client = new WebClient())
            {
                client.Headers[HttpRequestHeader.ContentType] = "application/json";

                try
                {
                    var result = client.UploadString(SmartWalkUrl, json);
                    Console.WriteLine(result);

                    var response = JsonConvert.DeserializeObject<Response>(result);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            Console.WriteLine("\nVenue View");
            var venueViewRequest = CreateVenueViewRequest(222, new[] {333, 444, 555});
            json = JsonConvert.SerializeObject(
                venueViewRequest,
                new JsonSerializerSettings
                    {
                        Formatting = Formatting.Indented,
                        NullValueHandling = NullValueHandling.Ignore
                    });
            Console.WriteLine(json);    

            Console.ReadLine();
        }

        public static Request CreateHomeViewRequest(string country, string state, string city)
        {
            var result = new Request
                {
                    Selects = new[]
                        {
                            new RequestSelect
                                {
                                    Fields = new[] {"Host", "Title", "StartTime", "Region"},
                                    From = RequestSelectFromTables.EventMetadata,
                                    As = "em",
                                    Where = new[]
                                        {
                                            new RequestSelectWhere
                                                {
                                                    Field = "Region.Country",
                                                    Operator = RequestSelectWhereOperators.EqualsTo,
                                                    Value = country
                                                },
                                            new RequestSelectWhere
                                                {
                                                    Field = "Region.State",
                                                    Operator = RequestSelectWhereOperators.EqualsTo,
                                                    Value = state
                                                },
                                            new RequestSelectWhere
                                                {
                                                    Field = "Region.City",
                                                    Operator = RequestSelectWhereOperators.EqualsTo,
                                                    Value = city
                                                }
                                        }
                                },
                            new RequestSelect
                                {
                                    Fields = new[] {"Name", "Picture"},
                                    From = RequestSelectFromTables.Entity,
                                    Where = new[]
                                        {
                                            new RequestSelectWhere
                                                {
                                                    Field = "Id",
                                                    Operator = RequestSelectWhereOperators.EqualsTo,
                                                    SelectValue = new RequestSelectWhereSelectValue
                                                        {
                                                            Field = "Host.Id",
                                                            SelectName = "em"
                                                        }
                                                }
                                        }
                                }
                        },
                    Storages = new[] {Storage.SmartWalk}
                };

            return result;
        }

        public static Request CreateEventViewRequest(int eventMetadataId)
        {
            var result = new Request
                {
                    Selects = new[]
                        {
                            new RequestSelect
                                {
                                    Fields = new[] {"Host", "Title", "StartTime", "Shows"},
                                    From = RequestSelectFromTables.EventMetadata,
                                    As = "em",
                                    Where = new[]
                                        {
                                            new RequestSelectWhere
                                                {
                                                    Field = "Id",
                                                    Operator = RequestSelectWhereOperators.EqualsTo,
                                                    Value = eventMetadataId
                                                }
                                        }
                                },
                            new RequestSelect
                                {
                                    Fields = new[]
                                        {
                                            "Venue", "IsReference", "Title", "Description", "StartTime", "EndTime",
                                            "Picture", "DetailsUrl"
                                        },
                                    From = RequestSelectFromTables.Show,
                                    As = "s",
                                    Where = new[]
                                        {
                                            new RequestSelectWhere
                                                {
                                                    Field = "Id",
                                                    Operator = RequestSelectWhereOperators.EqualsTo,
                                                    SelectValue = new RequestSelectWhereSelectValue
                                                        {
                                                            Field = "Shows",
                                                            SelectName = "em"
                                                        }
                                                }
                                        }
                                },
                            new RequestSelect
                                {
                                    Fields = new[] {"Name", "Picture", "Addresses"},
                                    From = RequestSelectFromTables.Entity,
                                    Where = new[]
                                        {
                                            new RequestSelectWhere
                                                {
                                                    Field = "Id",
                                                    Operator = RequestSelectWhereOperators.EqualsTo,
                                                    SelectValue = new RequestSelectWhereSelectValue
                                                        {
                                                            Field = "Venue.Id",
                                                            SelectName = "s"
                                                        }
                                                }
                                        }
                                }
                        },
                    Storages = new[] {Storage.SmartWalk}
                };

            return result;
        }

        public static Request CreateVenueViewRequest(int venueId, int[] showIds)
        {
            var result = new Request
                {
                    Selects = new[]
                        {
                            new RequestSelect
                                {
                                    Fields = new[] {"Name", "Description", "Picture", "Contacts", "Addresses"},
                                    From = RequestSelectFromTables.Entity,
                                    Where = new[]
                                        {
                                            new RequestSelectWhere
                                                {
                                                    Field = "Id",
                                                    Operator = RequestSelectWhereOperators.EqualsTo,
                                                    Value = venueId
                                                }
                                        }
                                },
                            new RequestSelect
                                {
                                    Fields =
                                        new[] {"Title", "Description", "StartTime", "EndTime", "Picture", "DetailsUrl"},
                                    From = RequestSelectFromTables.Show,
                                    Where = new[]
                                        {
                                            new RequestSelectWhere
                                                {
                                                    Field = "Id",
                                                    Operator = RequestSelectWhereOperators.EqualsTo,
                                                    Values = showIds.Cast<object>().ToArray()
                                                }
                                        }
                                }
                        }
                };

            return result;
        }
    }
}