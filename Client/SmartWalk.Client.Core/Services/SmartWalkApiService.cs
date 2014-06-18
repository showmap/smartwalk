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

        public async Task<OrgEvent[]> GetOrgEvents(Location location, DataSource source)
        {
            var request = SmartWalkApiFactory.CreateOrgEventsRequest(location);
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
            var request = SmartWalkApiFactory.CreateOrgEventRequest(id);
            var response = await GetResponse(request, source);
            var result = default(OrgEvent);

            if (response != null)
            {
                // reset event venues cache, if event is refreshed from server
                // this is to keep views' content synced
                if (source == DataSource.Server)
                {
                    var venuesRequest = 
                        SmartWalkApiFactory.CreateOrgEventVenuesRequest(id);
                    _cacheService.InvalidateString(GenerateKey(venuesRequest));
                }

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

        public async Task<Venue[]> GetOrgEventVenues(int id, DataSource source)
        {
            var request = SmartWalkApiFactory.CreateOrgEventVenuesRequest(id);
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

        public async Task<OrgEvent> GetOrgEventInfo(int id, DataSource source)
        {
            var request = SmartWalkApiFactory.CreateOrgEventInfoRequest(id);
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

        public async Task<Org> GetHost(int id, DataSource source)
        {
            var request = SmartWalkApiFactory.CreateHostRequest(id);
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

        private async Task<Response> GetResponse(Request request, DataSource source)
        {
            var result = default(Response);
            var key = GenerateKey(request);
            var isConnected = source != DataSource.Cache && 
                await _reachabilityService.GetIsReachable();

            if (!isConnected || source != DataSource.Server)
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
                (request != null 
                    ? request.GetHashCode().ToString() 
                    : string.Empty);
        }

        private static Venue CreateVenue(Entity entity, Show[] shows)
        {
            entity.Type = EntityType.Venue; // saving a bit traffic here

            var venueShows = 
                shows
                    .Where(
                        s => 
                            entity.Id == s.Venue.Id() && 
                            s.IsReference != true)
                    .ToArray();
            var result = new Venue(entity) 
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