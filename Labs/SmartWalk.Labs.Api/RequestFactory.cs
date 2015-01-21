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
                                    Offset = 0,
                                    Fetch = 5,
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
                                    Fields = new[] {"Host", "Title", "StartTime", 
                                        "VenueOrderType", "VenueTitleFormatType", "Shows"},
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

        public static Request CreateVenuesViewRequest(int eventId)
        {
            var result = new Request
                {
                    Selects = new[]
                        {
                            new RequestSelect
                                {
                                    Fields = new[] {"Shows"},
                                    From = RequestSelectFromTables.EventMetadata,
                                    As = "em",
                                    Where = new[]
                                        {
                                            new RequestSelectWhere
                                                {
                                                    Field = "Id",
                                                    Operator = RequestSelectWhereOperators.EqualsTo,
                                                    Value = eventId
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
                                        },
                                    SortBy = new[]
                                        {
                                            new RequestSelectSortBy
                                                {
                                                    Field = "StartTime"
                                                }
                                        }
                                },
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
                                },
                            new RequestSelect
                                {
                                    Fields = new[] {"Venue", "SortOrder", "Description"},
                                    From = RequestSelectFromTables.EventVenueDetail,
                                    Where = new[]
                                        {
                                            new RequestSelectWhere
                                                {
                                                    Field = "Event.Id",
                                                    Operator = RequestSelectWhereOperators.EqualsTo,
                                                    Value = eventId
                                                },
                                            new RequestSelectWhere
                                                {
                                                    Field = "Venue.Id",
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