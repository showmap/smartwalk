using System.Linq;
using SmartWalk.Shared.DataContracts;
using SmartWalk.Shared.DataContracts.Api;

namespace SmartWalk.Labs.Api
{
    public static class RequestFactory
    {
        public static Request CreateHomeViewRequest(double latitude, double longitude)
        {
            var result = new Request
                {
                    Selects = new[]
                        {
                            new RequestSelect
                                {
                                    Offset = 1,
                                    Fetch = 3,
                                    Fields = new[] {"host", "title", "picture", "startTime", "latitude", "longitude"},
                                    From = RequestSelectFromTables.GroupedEventMetadata.ToLowerInvariant(),
                                    As = "em",
                                    SortBy = new[]
                                        {
                                            new RequestSelectSortBy
                                                {
                                                    Field = "Latitude",
                                                    OfDistance = latitude,
                                                },
                                            new RequestSelectSortBy
                                                {
                                                    Field = "Longitude",
                                                    OfDistance = longitude
                                                },
                                            new RequestSelectSortBy
                                                {
                                                    Field = "startTime",
                                                    IsDescending = true
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
                                        },
                                    SortBy = new[]
                                        {
                                            new RequestSelectSortBy
                                                {
                                                    Field = "name"
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

        public static Request CreateHostViewRequest(int hostId)
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
                                                    Value = hostId
                                                }
                                        }
                                },
                            new RequestSelect
                                {
                                    Fields =
                                        new[] {"Host", "Title", "Picture", "StartTime" },
                                    From = RequestSelectFromTables.EventMetadata,
                                    Where = new[] {
                                        new RequestSelectWhere {
                                            Field = "Host.Id",
                                            Operator = RequestSelectWhereOperators.EqualsTo,
                                            Value = hostId
                                        }
                                    },
                                    SortBy = new[]
                                        {
                                            new RequestSelectSortBy
                                                {
                                                    Field = "StartTime",
                                                    IsDescending = true
                                                }
                                        }
                                }
                        }
            };

            return result;
        }
    }
}