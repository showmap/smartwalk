using SmartWalk.Shared.DataContracts;
using SmartWalk.Shared.DataContracts.Api;

namespace SmartWalk.Labs.Api
{
    public static class OldRequestFactory
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
                                    PictureSize = PictureSize.Medium,
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
                                    PictureSize = PictureSize.Medium,
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
                                            "Pictures", "DetailsUrl"
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

	public static class NewRequestFactory
	{
		public static Request CreateOrgEventsRequest()
		{
			var request = new Request {
				Selects = new[] {
					new RequestSelect {
						Offset = 0,
						Fetch = 25,
						Fields = new[] { "Host", "Title", "Picture", "StartTime" },
						PictureSize = PictureSize.Medium,
						From = RequestSelectFromTables.GroupedEventMetadata,
						As = "em",
						SortBy = new[] {
							new RequestSelectSortBy {
								Field = "StartTime",
								IsDescending = true
							}
						}
					},
					new RequestSelect {
						Fields = new[] { "Name", "Picture" },
						PictureSize = PictureSize.Medium,
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
						Fields = new[] { "Title", "Description", "Host", "StartTime", "EndTime", 
							"Latitude", "Longitude", "VenueOrderType", "VenueTitleFormatType", "Shows"
						},
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
						Fields = new[] { "Name" },
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
					},
					new RequestSelect {
						Fields = new[] {
							"Venue", "IsReference", "Title",
							"Description", "StartTime", "EndTime",
							"Pictures", "DetailsUrl"
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
						},
						SortBy = new [] {
							new RequestSelectSortBy {
								Field = "StartTime"
							}
						}
					},
					new RequestSelect {
						Fields = new[] { "Venue", "SortOrder" },
						From = RequestSelectFromTables.EventVenueDetail,
						Where = new[] {
							new RequestSelectWhere {
								Field = "Event.Id",
								Operator = RequestSelectWhereOperators.EqualsTo,
								Value = id
							},
							new RequestSelectWhere {
								Field = "Venue.Id",
								Operator = RequestSelectWhereOperators.EqualsTo,
								SelectValue = new RequestSelectWhereSelectValue {
									Field = "Venue.Id",
									SelectName = "s"
								}
							}
						}
					},
					new RequestSelect {
						Fields = new[] { "Name", "Addresses" },
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
						Fields = new[] { "VenueOrderType", "VenueTitleFormatType", "Shows" },
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
							"Pictures", "DetailsUrl"
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
						},
						SortBy = new [] {
							new RequestSelectSortBy {
								Field = "StartTime"
							}
						}
					},
					new RequestSelect {
						Fields = new[] { "Venue", "SortOrder", "Description" },
						From = RequestSelectFromTables.EventVenueDetail,
						Where = new[] {
							new RequestSelectWhere {
								Field = "Event.Id",
								Operator = RequestSelectWhereOperators.EqualsTo,
								Value = id
							},
							new RequestSelectWhere {
								Field = "Venue.Id",
								Operator = RequestSelectWhereOperators.EqualsTo,
								SelectValue = new RequestSelectWhereSelectValue {
									Field = "Venue.Id",
									SelectName = "s"
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
						Fields = new[] { "Host", "Title", "StartTime", "EndTime" },
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