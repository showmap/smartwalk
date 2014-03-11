using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using Orchard.Data;
using SmartWalk.Server.Records;
using SmartWalk.Server.Services.HostService;
using SmartWalk.Server.Services.VenueService;
using SmartWalk.Server.ViewModels;

namespace SmartWalk.Server.Services.EventService
{
    public class EventService : IEventService
    {
        private readonly IRepository<RegionRecord> _regionRepository;

        private readonly IHostService _hostService;
        private readonly IVenueService _venueService;

        public EventService(IRepository<RegionRecord> regionRepository,
            IHostService hostService, IVenueService venueService)
        {
            _regionRepository = regionRepository;

            _hostService = hostService;
            _venueService = venueService;
        }

        public IList<EventMetadataVm> GetUserEvents(SmartWalkUserRecord user) {
            return user.EventMetadataRecords.Select(ViewModelContractFactory.CreateViewModelContract).ToList();
        }

        public EventMetadataFullVm GetUserEventVmById(SmartWalkUserRecord user, int id) {
            var eventMetadata = user.EventMetadataRecords.FirstOrDefault(u => u.Id == id);
            if (eventMetadata != null) {
                return new EventMetadataFullVm {
                    EventMetadata = ViewModelContractFactory.CreateViewModelContract(eventMetadata),
                    Hosts = _hostService.GetUserHosts(user),
                    Regions = _regionRepository.Table.Select(ViewModelContractFactory.CreateViewModelContract).ToList(),
                    Shows = eventMetadata.ShowRecords.Select(ViewModelContractFactory.CreateViewModelContract).ToList(),
                    Venues = _venueService.GetUserVenues(user)
                };
            }

            return null;
        }        
    }
}