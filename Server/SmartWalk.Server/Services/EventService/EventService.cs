using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using Orchard.Data;
using SmartWalk.Server.Records;
using SmartWalk.Server.Services.EntityService;
using SmartWalk.Server.ViewModels;

namespace SmartWalk.Server.Services.EventService
{
    public class EventService : IEventService
    {
        private readonly IEntityService _entityService;

        public EventService(IEntityService entityService)
        {
            _entityService = entityService;
        }

        public IList<EventMetadataVm> GetUserEvents(SmartWalkUserRecord user) {
            return user.EventMetadataRecords.Select(ViewModelContractFactory.CreateViewModelContract).ToList();
        }

        public EventMetadataFullVm GetUserEventVmById(SmartWalkUserRecord user, int id) {
            var eventMetadata = user.EventMetadataRecords.FirstOrDefault(u => u.Id == id);
            if (eventMetadata != null) {
                return new EventMetadataFullVm {
                    EventMetadata = ViewModelContractFactory.CreateViewModelContract(eventMetadata),
                    Hosts = _entityService.GetUserEntities(user, EntityType.Host),
                    Shows = eventMetadata.ShowRecords.Select(ViewModelContractFactory.CreateViewModelContract).ToList(),
                    Venues = _entityService.GetUserEntities(user, EntityType.Venue)
                };
            }

            return null;
        }        
    }
}