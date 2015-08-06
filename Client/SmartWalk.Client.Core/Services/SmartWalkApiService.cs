using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SmartWalk.Client.Core.Model;
using SmartWalk.Client.Core.Model.DataContracts;
using SmartWalk.Client.Core.Utils;
using SmartWalk.Shared.DataContracts;
using SmartWalk.Shared.DataContracts.Api;

namespace SmartWalk.Client.Core.Services
{
    public class SmartWalkApiService : ISmartWalkApiService
    {
        private const string KeyPrefix = "api";

        private readonly IHttpService _httpService;
        private readonly ICacheService _cacheService;
        private readonly IReachabilityService _reachabilityService;
        private readonly IConfiguration _configuration;

        public SmartWalkApiService(
            IHttpService httpService,
            ICacheService cacheService,
            IReachabilityService reachabilityService,
            IConfiguration configuration)
        {
            _httpService = httpService;
            _cacheService = cacheService;
            _reachabilityService = reachabilityService;
            _configuration = configuration;
        }

        public async Task<IApiResult<OrgEvent[]>> GetOrgEvents(Location location, DataSource source)
        {
            var request = SmartWalkApiFactory.CreateOrgEventsRequest(location);
            var response = await GetResponse(request, source);
            var result = default(IApiResult<OrgEvent[]>);

            if (response != null && response.Data != null)
            {
                var eventMetadatas = response.Data
                    .GetRecords<EventMetadata>(0)
                    .ToArray();

                var hosts = response.Data
                    .GetRecords<Entity>(1)
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
                var eventMetadata = response.Data
                    .GetRecords<EventMetadata>(0)
                    .FirstOrDefault();

                var host = response.Data
                    .GetRecords<Entity>(1)
                    .FirstOrDefault();

                var shows = response.Data
                    .GetRecords<Show>(2)
                    .ToArray();

                var venueDetails = response.Data
                    .GetRecords<EventVenueDetail>(3)
                    .ToArray();

                var venues = response.Data
                    .GetRecords<Entity>(4)
                    .Select(e =>
                        {
                            var venue = CreateVenue(e, venueDetails, shows);
                            return venue;
                        })
                    .OrderBy(venueDetails, eventMetadata.VenueOrderType)
                    .ToArray();

                var orgEvent = new OrgEvent(eventMetadata, host, venues);

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
                var eventMetadata = response.Data
                    .GetRecords<EventMetadata>(0)
                    .FirstOrDefault();

                var shows = response.Data
                    .GetRecords<Show>(1)
                    .ToArray();

                var venueDetails = response.Data
                    .GetRecords<EventVenueDetail>(2)
                    .ToArray();

                var venues = response.Data
                    .GetRecords<Entity>(3)
                    .Select(e =>
                        {
                            var venue = CreateVenue(e, venueDetails, shows);
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
                var eventMetadata = response.Data
                    .GetRecords<EventMetadata>(0)
                    .FirstOrDefault();

                var host = response.Data
                    .GetRecords<Entity>(1)
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

            if (response != null && response.Data != null && response.Data.Selects != null)
            {
                var entity = response.Data
                    .GetRecords<Entity>(0)
                    .FirstOrDefault();

                var eventMetadatas = response.Data
                    .GetRecords<EventMetadata>(1)
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
                request.ClientVersion = _configuration.ClientVersion;

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
            var venueShows = shows
                    .Where(s => entity.Id == s.Venue.Id() && s.IsReference != true)
                    .ToArray();

            for (var i = 0; i < venueShows.Length; i++)
            {
                var show = venueShows[i];
                var nextShow = i + 1 < venueShows.Length ? venueShows[i + 1] : null;
                show.Status = show.GetStatus(nextShow);
            }

            var result = 
                new Venue(entity,
                    venueDetail != null ? venueDetail.Description : null) {
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

    public static class SmartWalkApiExtensions
    {
        public static IEnumerable<T> GetRecords<T>(this Response response, int index)
        {
            return response.Selects != null && index < response.Selects.Length 
                ? (response.Selects[index].Records
                    .Cast<JObject>()
                    .Select(jo => jo.ToObject<T>()) ?? Enumerable.Empty<T>()) 
                    : Enumerable.Empty<T>();
        }
    }
}