using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using RestSharp.Portable;
using SmartWalk.Shared.DataContracts;
using SmartWalk.Shared.DataContracts.Api;
using SmartWalk.Client.Core.Model;
using SmartWalk.Client.Core.Model.DataContracts;
using SmartWalk.Client.Core.Utils;

namespace SmartWalk.Client.Core.Services
{
    public class SmartWalkApiService : ISmartWalkApiService
    {
        private const string apiUrl = "http://smartwalk.azurewebsites.net/api";

        private readonly IReachabilityService _reachabilityService;

        public SmartWalkApiService(IReachabilityService reachabilityService)
        {
            _reachabilityService = reachabilityService;
        }

        public async Task<OrgEvent[]> GetOrgEvents(Location location, DataSource source)
        {
            var request = new Request {
                Selects = new[] {
                    new RequestSelect {
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

            var response = await GetResponse(request);

            var eventMetadatas = response
                .Selects[0].Records
                .Cast<JObject>()
                .Select(r => r.ToObject<EventMetadata>())
                .ToArray();

            var hosts = response
                .Selects[1].Records
                .Cast<JObject>()
                .Select(r => r.ToObject<Entity>())
                .ToArray();

            var result = eventMetadatas
                .Select(em => 
                    new OrgEvent(
                        em, 
                        hosts.First(h => h.Id == em.Host.Id())))
                .ToArray();

            return result;
        }

        public async Task<OrgEvent> GetOrgEvent(int id, DataSource source)
        {
            var request = new Request {
                Selects = new[] {
                    new RequestSelect {
                        Fields = new[] { "Host", "Title", "StartTime", "EndTime", "Shows" },
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

            var response = await GetResponse(request);

            var eventMetadata = response
                .Selects[0].Records
                .Cast<JObject>()
                .Select(em => em.ToObject<EventMetadata>())
                .FirstOrDefault();

            var shows = response
                .Selects[1].Records
                .Cast<JObject>()
                .Select(s => s.ToObject<Show>())
                .ToArray();

            var venues = response
                .Selects[2].Records
                .Cast<JObject>()
                .Select(e => {
                    var entity = e.ToObject<Entity>();
                    var venue = CreateVenue(entity, shows);
                    return venue;
                })
                .ToArray();

            var result = new OrgEvent(eventMetadata, null, venues);
            return result;
        }

        public async Task<Venue[]> GetOrgEventVenues(int id, DataSource source)
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

            var response = await GetResponse(request);

            var shows = response
                .Selects[1].Records
                .Cast<JObject>()
                .Select(s => s.ToObject<Show>())
                .ToArray();

            var venues = response
                .Selects[2].Records
                .Cast<JObject>()
                .Select(e => {
                    var entity = e.ToObject<Entity>();
                    var venue = CreateVenue(entity, shows);
                    return venue;
                })
                .ToArray();

            return venues;
        }

        public Task<Org> GetHost(int id, DataSource source)
        {
            throw new NotImplementedException();
        }

        private async Task<Response> GetResponse(Request request)
        {
            var client = new RestClient(new Uri(apiUrl));
            var restRequest = new RestRequest(string.Empty, HttpMethod.Post);
            restRequest.AddBody(request);
            var result = await client.Execute<Response>(restRequest);
            return result.Data;
        }

        private bool GetIsConnected()
        {
            return _reachabilityService.IsHostReachable(apiUrl);
        }

        private static Venue CreateVenue(Entity entity, Show[] shows)
        {
            var venueShows = shows
                .Where(s => entity.Id == s.Venue.Id() && s.IsReference != true)
                .ToArray();
            var result = new Venue(entity) { Shows = venueShows };
            return result;
        }
    }
}