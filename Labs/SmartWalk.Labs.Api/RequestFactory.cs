using System.Linq;
using SmartWalk.Shared.DataContracts;
using SmartWalk.Shared.DataContracts.Api;

namespace SmartWalk.Labs.Api
{
    public static class RequestFactory
    {
        public static Request CreateHomeViewRequest(string country, string state, string city)
        {
            var result = new Request
                {
                    Selects = new[]
                        {
                            new RequestSelect
                                {
                                    Fields = new[] {"host", "title", "startTime", "region"},
                                    From = RequestSelectFromTables.EventMetadata.ToLowerInvariant(),
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
                                                    Field = "region.State",
                                                    Operator = RequestSelectWhereOperators.EqualsTo,
                                                    Value = state
                                                },
                                            new RequestSelectWhere
                                                {
                                                    Field = "Region.city",
                                                    Operator = RequestSelectWhereOperators.EqualsTo,
                                                    Value = city
                                                }
                                        }
                                },
                            new RequestSelect
                                {
                                    Fields = new[] {"name", "picture"},
                                    From = RequestSelectFromTables.Entity,
                                    Where = new[]
                                        {
                                            new RequestSelectWhere
                                                {
                                                    Field = "id",
                                                    Operator = RequestSelectWhereOperators.EqualsTo,
                                                    SelectValue = new RequestSelectWhereSelectValue
                                                        {
                                                            Field = "Host.id",
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
                                            "Venue", "IsReference", "Title", 
                                            "Description", "StartTime", "EndTime",
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
                                                            Field = "Shows.Id",
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
                    Storages = new[] {Storage.SmartWalk.ToLower()}
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
