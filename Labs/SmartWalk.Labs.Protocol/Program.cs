using System;
using Newtonsoft.Json;
using SmartWalk.Shared.DataContracts;
using SmartWalk.Shared.DataContracts.Protocol;

namespace SmartWalk.Labs.Protocol
{
    public class MainClass
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("SmartWalk Protocol JSON objects demo");

            Console.WriteLine("\nHome View");

            var region = new Region { Country = "USA", State = "California", City = "San Francisco" };
            var homeViewRequest = CreateHomeViewRequest(region);
            var json = JsonConvert.SerializeObject(
                homeViewRequest, 
                new JsonSerializerSettings {
                    Formatting = Formatting.Indented,
                    NullValueHandling = NullValueHandling.Ignore
                });
            Console.WriteLine(json);

            Console.WriteLine("\nEvent View");
            var eventViewRequest = CreateEventViewRequest("111");
            json = JsonConvert.SerializeObject(
                eventViewRequest, 
                new JsonSerializerSettings {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore
            });
            Console.WriteLine(json);

            Console.WriteLine("\nVenue View");
            var venueViewRequest = CreateVenueViewRequest("222", new [] {"333", "444", "555"});
            json = JsonConvert.SerializeObject(
                venueViewRequest, 
                new JsonSerializerSettings {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore
            });
            Console.WriteLine(json);


            Console.ReadLine();
        }

        public static Request CreateHomeViewRequest(IRegion region)
        {
            var result = new Request {
                Selects = new [] { 
                    new RequestSelect {
                        Fields = new [] { "Host", "Title", "StartTime", "Region" },
                        From = RequestSelectFromTables.EventMetadata,
                        As = "em",
                        Where = new RequestSelectWhere {
                            Field = "Region",
                            Operator = RequestSelectWhereOperators.EqualsTo,
                            Value = region
                        }
                    }, 
                    new RequestSelect { 
                        Fields = new [] { "Name", "Picture" },
                        From = RequestSelectFromTables.Entity,
                        Where = new RequestSelectWhere {
                            Field = "Id",
                            Operator = RequestSelectWhereOperators.EqualsTo,
                            Value = new RequestSelectWhereSelectValue {
                                Field = "Host",
                                SelectName = "em"
                            }
                        }
                    }
                },
                Storages = new [] { Storage.SmartWalk }
            };

            return result;
        }

        public static Request CreateEventViewRequest(string eventMetadataId)
        {
            var result = new Request {
                Selects = new [] { 
                    new RequestSelect {
                        Fields = new [] { "Host", "Title", "StartTime", "Shows" },
                        From = RequestSelectFromTables.EventMetadata,
                        As = "em",
                        Where = new RequestSelectWhere {
                            Field = "Id",
                            Operator = RequestSelectWhereOperators.EqualsTo,
                            Value = eventMetadataId
                        }
                    }, 
                    new RequestSelect {
                        Fields = new [] { "Venue", "IsReference", "Title", "Description", "StartTime", "EndTime", "Picture", "DetailsUrl" },
                        From = RequestSelectFromTables.Show,
                        As = "s",
                        Where = new RequestSelectWhere {
                            Field = "Id",
                            Operator = RequestSelectWhereOperators.EqualsTo,
                            Value = new RequestSelectWhereSelectValue {
                                Field = "Shows",
                                SelectName = "em"
                            }
                        }
                    },
                    new RequestSelect { 
                        Fields = new [] { "Name", "Picture", "Addresses" },
                        From = RequestSelectFromTables.Entity,
                        Where = new RequestSelectWhere {
                            Field = "Id",
                            Operator = RequestSelectWhereOperators.EqualsTo,
                            Value = new RequestSelectWhereSelectValue {
                                Field = "Venue",
                                SelectName = "s"
                            }
                        }
                    }
                },
                Storages = new [] { Storage.SmartWalk }
            };

            return result;
        }

        public static Request CreateVenueViewRequest(string venueId, string[] showIds)
        {
            var result = new Request {
                Selects = new [] { 
                    new RequestSelect { 
                        Fields = new [] { "Name", "Description", "Picture", "Contacts", "Addresses" },
                        From = RequestSelectFromTables.Entity,
                        Where = new RequestSelectWhere {
                            Field = "Id",
                            Operator = RequestSelectWhereOperators.EqualsTo,
                            Value = venueId
                        }
                    },
                    new RequestSelect {
                        Fields = new [] { "Title", "Description", "StartTime", "EndTime", "Picture", "DetailsUrl" },
                        From = RequestSelectFromTables.Show,
                        Where = new RequestSelectWhere {
                            Field = "Id",
                            Operator = RequestSelectWhereOperators.EqualsTo,
                            Value = showIds
                        }
                    },
                }
            };

            return result;
        }
    }
}