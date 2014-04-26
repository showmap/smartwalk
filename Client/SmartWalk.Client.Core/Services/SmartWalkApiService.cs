using System;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SmartWalk.Shared.DataContracts;
using SmartWalk.Shared.DataContracts.Api;
using SmartWalk.Client.Core.Model;
using SmartWalk.Client.Core.Model.DataContracts;
using SmartWalk.Client.Core.Utils;

namespace SmartWalk.Client.Core.Services
{
    public class SmartWalkApiService : ISmartWalkApiService
    {
        private const string KeyPrefix = "api";

        private readonly IConfiguration _configuration;
        private readonly IHttpService _httpService;
        private readonly ICacheService _cacheService;
        private readonly IReachabilityService _reachabilityService;

        public SmartWalkApiService(
            IConfiguration configuration,
            IHttpService httpService,
            ICacheService cacheService,
            IReachabilityService reachabilityService)
        {
            _configuration = configuration;
            _httpService = httpService;
            _cacheService = cacheService;
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

            var response = await GetResponse(request, source);
            var result = default(OrgEvent[]);

            if (response != null)
            {
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

                result = eventMetadatas
                    .Select(em => 
                        new OrgEvent(
                            em, 
                            hosts.First(h => h.Id == em.Host.Id())))
                    .ToArray();
            }

            return result;
        }

        public async Task<OrgEvent> GetOrgEvent(int id, DataSource source)
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

            var response = await GetResponse(request, source);
            var result = default(OrgEvent);

            if (response != null)
            {
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
                    .Select(e =>
                        {
                            var entity = e.ToObject<Entity>();
                            var venue = CreateVenue(entity, shows);
                            return venue;
                        })
                    .ToArray();

                result = new OrgEvent(eventMetadata, null, venues);
            }

            return result;
        }

        public async Task<OrgEvent> GetOrgEventInfo(int id, DataSource source)
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

            var response = await GetResponse(request, source);
            var result = default(OrgEvent);

            if (response != null)
            {
                var eventMetadata = response
                    .Selects[0].Records
                    .Cast<JObject>()
                    .Select(em => em.ToObject<EventMetadata>())
                    .FirstOrDefault();

                var host = response
                    .Selects[1].Records
                    .Cast<JObject>()
                    .Select(e => e.ToObject<Entity>())
                    .FirstOrDefault();

                result = new OrgEvent(eventMetadata, host);
            }

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

            var response = await GetResponse(request, source);
            var result = default(Venue[]);

            if (response != null)
            {
                var shows = response
                    .Selects[1].Records
                    .Cast<JObject>()
                    .Select(s => s.ToObject<Show>())
                    .ToArray();

                result = response
                    .Selects[2].Records
                    .Cast<JObject>()
                    .Select(e =>
                        {
                            var entity = e.ToObject<Entity>();
                            var venue = CreateVenue(entity, shows);
                            return venue;
                        })
                    .ToArray();
            }

            return result;
        }

        public async Task<Org> GetHost(int id, DataSource source)
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

            var response = await GetResponse(request, source);
            var result = default(Org);

            if (response != null)
            {
                var entity = response
                    .Selects[0].Records
                    .Cast<JObject>()
                    .Select(e => e.ToObject<Entity>())
                    .FirstOrDefault();

                var eventMetadatas = response
                    .Selects[1].Records
                    .Cast<JObject>()
                    .Select(r => r.ToObject<EventMetadata>())
                    .ToArray();

                if (entity != null)
                {
                    result = CreateOrg(entity, eventMetadatas);
                }
            }

            return result;
        }

        private bool GetIsConnected()
        {
            return _reachabilityService.IsHostReachable(_configuration.Host);
        }

        private async Task<Response> GetResponse(Request request, DataSource source)
        {
            var result = default(Response);
            var isConnected = GetIsConnected();
            var key = GenerateKey(request);

            if (!isConnected || source == DataSource.Cache)
            {
                result = _cacheService.GetObject<Response>(key);
            }

            if (isConnected && result == null)
            {
                result = await _httpService.Get<Response>(request);

                if (result != null)
                {
                    _cacheService.SetObject(key, result);
                }
            }

            return result;
        }

        private static string GenerateKey(Request request)
        {
            return KeyPrefix + 
                request != null 
                    ? request.GetHashCode().ToString() 
                    : string.Empty;
        }

        private static Venue CreateVenue(Entity entity, Show[] shows)
        {
            var venueShows = shows
                .Where(s => entity.Id == s.Venue.Id() && s.IsReference != true)
                .ToArray();
            var result = new Venue(entity) { Shows = venueShows };
            return result;
        }

        private static Org CreateOrg(Entity entity, EventMetadata[] eventMetadatas)
        {
            var orgEvents = eventMetadatas
                .Select(em => new OrgEvent(em, entity))
                .ToArray();
            var result = new Org(entity) { OrgEvents = orgEvents };
            return result;
        }
    }
}