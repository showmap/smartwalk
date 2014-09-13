using SmartWalk.Shared.DataContracts;
using SmartWalk.Shared.DataContracts.Api;
using SmartWalk.Client.Core.Model;

namespace SmartWalk.Client.Core.Services
{
    public static class SmartWalkApiFactory
    {
        public static Request CreateOrgEventsRequest(Location location)
        {
            var request = new Request {
                Selects = new[] {
                    new RequestSelect {
                        Offset = 0,
                        Fetch = 25,
                        Fields = new[] { "Host", "Title", "Picture", "StartTime" },
                        From = RequestSelectFromTables.GroupedEventMetadata,
                        As = "em",
                        SortBy = new[] {
                            new RequestSelectSortBy {
                                Field = "Latitude",
                                OfDistance = location.Latitude,
                            },
                            new RequestSelectSortBy {
                                Field = "Longitude",
                                OfDistance = location.Longitude
                            },
                            new RequestSelectSortBy {
                                Field = "StartTime",
                                IsDescending = true
                            }
                        }
                    },
                    new RequestSelect {
                        Fields = new[] { "Name", "Picture" },
                        From = RequestSelectFromTables.Entity,
                        Where = new[] {
                            new RequestSelectWhere {
                                Field = "Id",
                                Operator = RequestSelectWhereOperators.EqualsTo,
                                SelectValue = new RequestSelectWhereSelectValue {
                                    Field = "Host.Id",
                                    SelectName = "em"
                                }
                            }
                        }
                    }
                },
                Storages = new[] { Storage.SmartWalk }
            };

            return request;
        }

        public static Request CreateOrgEventRequest(int id)
        {
            var request = new Request {
                Selects = new[] {
                    new RequestSelect {
                        Fields = new[] { "Host", "StartTime", "EndTime", "Shows" },
                        From = RequestSelectFromTables.EventMetadata,
                        As = "em",
                        Where = new[] {
                            new RequestSelectWhere {
                                Field = "Id",
                                Operator = RequestSelectWhereOperators.EqualsTo,
                                Value = id
                            }
                        }
                    },
                    new RequestSelect {
                        Fields = new[] {
                            "Venue", "IsReference", "Title",
                            "Description", "StartTime", "EndTime",
                            "Picture", "DetailsUrl"
                        },
                        From = RequestSelectFromTables.Show,
                        As = "s",
                        Where = new[] {
                            new RequestSelectWhere {
                                Field = "Id",
                                Operator = RequestSelectWhereOperators.EqualsTo,
                                SelectValue = new RequestSelectWhereSelectValue {
                                    Field = "Shows.Id",
                                    SelectName = "em"
                                }
                            }
                        }
                    },
                    new RequestSelect {
                        Fields = new[] { "Name", "Picture", "Addresses" },
                        From = RequestSelectFromTables.Entity,
                        Where = new[] {
                            new RequestSelectWhere {
                                Field = "Id",
                                Operator = RequestSelectWhereOperators.EqualsTo,
                                SelectValue = new RequestSelectWhereSelectValue {
                                    Field = "Venue.Id",
                                    SelectName = "s"
                                }
                            }
                        },
                        SortBy = new[] {
                            new RequestSelectSortBy {
                                Field = "Name"
                            }
                        }
                    }
                },
                Storages = new[] { Storage.SmartWalk }
            };

            return request;
        }

        public static Request CreateOrgEventInfoRequest(int id)
        {
            var request = new Request {
                Selects = new[] {
                    new RequestSelect {
                        Fields = new[] { "Host", "StartTime", "EndTime", 
                            "Title", "Description", "Picture", "Latitude", "Longitude" },
                        From = RequestSelectFromTables.EventMetadata,
                        As = "em",
                        Where = new[] {
                            new RequestSelectWhere {
                                Field = "Id",
                                Operator = RequestSelectWhereOperators.EqualsTo,
                                Value = id
                            }
                        }
                    },
                    new RequestSelect {
                        Fields = new[] { "Name", "Picture" },
                        From = RequestSelectFromTables.Entity,
                        Where = new[] {
                            new RequestSelectWhere {
                                Field = "Id",
                                Operator = RequestSelectWhereOperators.EqualsTo,
                                SelectValue = new RequestSelectWhereSelectValue {
                                    Field = "Host.Id",
                                    SelectName = "em"
                                }
                            }
                        }
                    }
                },
                Storages = new[] { Storage.SmartWalk }
            };

            return request;
        }

        public static Request CreateOrgEventVenuesRequest(int id)
        {
            var request = new Request {
                Selects = new[] {
                    new RequestSelect {
                        Fields = new[] { "Shows" },
                        From = RequestSelectFromTables.EventMetadata,
                        As = "em",
                        Where = new[] {
                            new RequestSelectWhere {
                                Field = "Id",
                                Operator = RequestSelectWhereOperators.EqualsTo,
                                Value = id
                            }
                        }
                    },
                    new RequestSelect {
                        Fields = new[] {
                            "Venue", "IsReference", "Title",
                            "Description", "StartTime", "EndTime",
                            "Picture", "DetailsUrl"
                        },
                        From = RequestSelectFromTables.Show,
                        As = "s",
                        Where = new[] {
                            new RequestSelectWhere {
                                Field = "Id",
                                Operator = RequestSelectWhereOperators.EqualsTo,
                                SelectValue = new RequestSelectWhereSelectValue {
                                    Field = "Shows.Id",
                                    SelectName = "em"
                                }
                            }
                        }
                    },
                    new RequestSelect {
                        Fields = new[] { "Name", "Description", "Picture", "Contacts", "Addresses" },
                        From = RequestSelectFromTables.Entity,
                        Where = new[] {
                            new RequestSelectWhere {
                                Field = "Id",
                                Operator = RequestSelectWhereOperators.EqualsTo,
                                SelectValue = new RequestSelectWhereSelectValue {
                                    Field = "Venue.Id",
                                    SelectName = "s"
                                }
                            }
                        },
                        SortBy = new[] {
                            new RequestSelectSortBy {
                                Field = "name"
                            }
                        }
                    }
                },
                Storages = new[] { Storage.SmartWalk }
            };

            return request;
        }

        public static Request CreateHostRequest(int id)
        {
            var request = new Request {
                Selects = new[] {
                    new RequestSelect {
                        Fields = new[] { "Name", "Description", "Picture", "Contacts", "Addresses" },
                        From = RequestSelectFromTables.Entity,
                        Where = new[] {
                            new RequestSelectWhere {
                                Field = "Id",
                                Operator = RequestSelectWhereOperators.EqualsTo,
                                Value = id
                            }
                        }
                    },
                    new RequestSelect {
                        Fields = new[] { "Host", "Title", "Picture", "StartTime" },
                        From = RequestSelectFromTables.EventMetadata,
                        Where = new[] {
                            new RequestSelectWhere {
                                Field = "Host.Id",
                                Operator = RequestSelectWhereOperators.EqualsTo,
                                Value = id
                            }
                        },
                        SortBy = new[] {
                            new RequestSelectSortBy {
                                Field = "StartTime",
                                IsDescending = true
                            }
                        }
                    },
                },
                Storages = new[] { Storage.SmartWalk }
            };

            return request;
        }
    }
}