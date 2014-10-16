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

        private readonly IHttpService _httpService;
        private readonly ICacheService _cacheService;
        private readonly IReachabilityService _reachabilityService;

        public SmartWalkApiService(
            IHttpService httpService,
            ICacheService cacheService,
            IReachabilityService reachabilityService)
        {
            _httpService = httpService;
            _cacheService = cacheService;
            _reachabilityService = reachabilityService;
        }

        public async Task<IApiResult<OrgEvent[]>> GetOrgEvents(Location location, DataSource source)
        {
            var request = SmartWalkApiFactory.CreateOrgEventsRequest(location);
            var response = await GetResponse(request, source);
            var result = default(IApiResult<OrgEvent[]>);

            if (response != null && response.Data != null)
            {
                var eventMetadatas = response
                    .Data
                    .Selects[0].Records
                    .Cast<JObject>()
                    .Select(r => r.ToObject<EventMetadata>())
                    .ToArray();

                var hosts = response
                    .Data
                    .Selects[1].Records
                    .Cast<JObject>()
                    .Select(r => r.ToObject<Entity>())
                    .ToArray();

                var orgEvents = eventMetadatas
                    .Select(em => 
                        new OrgEvent(em, hosts.First(h => h.Id == em.Host.Id())))
                    .ToArray();

                result = new ApiResult<OrgEvent[]> {
                    Data = orgEvents,
                    Source = response.Source
                };
            }

            return result;
        }

        public async Task<IApiResult<OrgEvent>> GetOrgEvent(int id, DataSource source)
        {
            var request = SmartWalkApiFactory.CreateOrgEventRequest(id);
            var response = await GetResponse(request, source);
            var result = default(IApiResult<OrgEvent>);

            if (response != null && response.Data != null)
            {
                var eventMetadata = response
                    .Data
                    .Selects[0].Records
                    .Cast<JObject>()
                    .Select(em => em.ToObject<EventMetadata>())
                    .FirstOrDefault();

                var shows = response
                    .Data
                    .Selects[1].Records
                    .Cast<JObject>()
                    .Select(s => s.ToObject<Show>())
                    .ToArray();

                var venueDetails = response
                    .Data
                    .Selects[2].Records
                    .Cast<JObject>()
                    .Select(s => s.ToObject<EventVenueDetail>())
                    .ToArray();

                var venues = response
                    .Data
                    .Selects[3].Records
                    .Cast<JObject>()
                    .Select(e =>
                        {
                            var entity = e.ToObject<Entity>();
                            var venue = CreateVenue(entity, venueDetails, shows);
                            return venue;
                        })
                    .OrderBy(venueDetails, eventMetadata.VenueOrderType)
                    .ToArray();

                var orgEvent = new OrgEvent(eventMetadata, null, venues);

                result = new ApiResult<OrgEvent> {
                    Data = orgEvent,
                    Source = response.Source
                };
            }

            return result;
        }

        public async Task<IApiResult<Venue[]>> GetOrgEventVenues(int id, DataSource source)
        {
            var request = SmartWalkApiFactory.CreateOrgEventVenuesRequest(id);
            var response = await GetResponse(request, source);
            var result = default(IApiResult<Venue[]>);

            if (response != null && response.Data != null)
            {
                var eventMetadata = response
                    .Data
                    .Selects[0].Records
                    .Cast<JObject>()
                    .Select(em => em.ToObject<EventMetadata>())
                    .FirstOrDefault();

                var shows = response
                    .Data
                    .Selects[1].Records
                    .Cast<JObject>()
                    .Select(s => s.ToObject<Show>())
                    .ToArray();

                var venueDetails = response
                    .Data
                    .Selects[2].Records
                    .Cast<JObject>()
                    .Select(s => s.ToObject<EventVenueDetail>())
                    .ToArray();

                var venues = response
                    .Data
                    .Selects[3].Records
                    .Cast<JObject>()
                    .Select(e =>
                        {
                            var entity = e.ToObject<Entity>();
                            var venue = CreateVenue(entity, venueDetails, shows);
                            return venue;
                        })
                    .OrderBy(venueDetails, eventMetadata.VenueOrderType)
                    .ToArray();

                result = new ApiResult<Venue[]> {
                    Data = venues,
                    Source = response.Source
                };
            }

            return result;
        }

        public async Task<IApiResult<OrgEvent>> GetOrgEventInfo(int id, DataSource source)
        {
            var request = SmartWalkApiFactory.CreateOrgEventInfoRequest(id);
            var response = await GetResponse(request, source);
            var result = default(IApiResult<OrgEvent>);

            if (response != null && response.Data != null)
            {
                var eventMetadata = response
                    .Data
                    .Selects[0].Records
                    .Cast<JObject>()
                    .Select(em => em.ToObject<EventMetadata>())
                    .FirstOrDefault();

                var host = response
                    .Data
                    .Selects[1].Records
                    .Cast<JObject>()
                    .Select(e => e.ToObject<Entity>())
                    .FirstOrDefault();

                var orgEvent = new OrgEvent(eventMetadata, host);

                result = new ApiResult<OrgEvent> {
                    Data = orgEvent,
                    Source = response.Source
                };
            }

            return result;
        }

        public async Task<IApiResult<Org>> GetHost(int id, DataSource source)
        {
            var request = SmartWalkApiFactory.CreateHostRequest(id);
            var response = await GetResponse(request, source);
            var result = default(IApiResult<Org>);

            if (response != null && response.Data != null)
            {
                var entity = response
                    .Data
                    .Selects[0].Records
                    .Cast<JObject>()
                    .Select(e => e.ToObject<Entity>())
                    .FirstOrDefault();

                var eventMetadatas = response
                    .Data
                    .Selects[1].Records
                    .Cast<JObject>()
                    .Select(r => r.ToObject<EventMetadata>())
                    .ToArray();

                if (entity != null)
                {
                    var org = CreateOrg(entity, eventMetadatas);

                    result = new ApiResult<Org> {
                        Data = org,
                        Source = response.Source
                    };
                }
            }

            return result;
        }

        private async Task<ApiResult<Response>> GetResponse(Request request, DataSource source)
        {
            var result = default(ApiResult<Response>);
            var key = GenerateKey(request);
            var isConnected = 
                source != DataSource.Cache &&
                await _reachabilityService.GetIsReachable();

            if (!isConnected || source != DataSource.Server)
            {
                result = new ApiResult<Response> { 
                    Data = _cacheService.GetObject<Response>(key), 
                    Source = DataSource.Cache
                };
            }

            if (isConnected && (result == null || result.Data == null))
            {
                var response = await _httpService.Get<Response>(request);
                if (response != null)
                {
                    _cacheService.SetObject(key, response);
                }

                result = new ApiResult<Response> { 
                    Data = response, 
                    Source = DataSource.Server
                };
            }

            return result;
        }

        private static string GenerateKey(Request request)
        {
            return KeyPrefix + 
                (request != null 
                    ? request.GetHashCode().ToString() 
                    : string.Empty);
        }

        private static Venue CreateVenue(Entity entity, EventVenueDetail[] venueDetails, Show[] shows)
        {
            entity.Type = EntityType.Venue; // saving a bit traffic here

            var venueDetail = venueDetails
                .FirstOrDefault(d => d.Venue.Id() == entity.Id);
            var venueShows = 
                shows
                    .Where(
                        s => 
                            entity.Id == s.Venue.Id() && 
                            s.IsReference != true)
                    .ToArray();
            var result = new Venue(
                    entity,
                    venueDetail != null ? venueDetail.Description : null) 
                {
                    Shows = venueShows.Length > 0 ? venueShows : null
                };
            return result;
        }

        private static Org CreateOrg(Entity entity, EventMetadata[] eventMetadatas)
        {
            entity.Type = EntityType.Host; // saving a bit traffic here

            var orgEvents = eventMetadatas
                .Select(em => new OrgEvent(em, entity))
                .ToArray();
            var result = new Org(entity) { OrgEvents = orgEvents };
            return result;
        }
    }
}