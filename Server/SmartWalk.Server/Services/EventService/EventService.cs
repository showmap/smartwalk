using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using Orchard;
using Orchard.Data;
using SmartWalk.Server.Records;
using SmartWalk.Server.Services.CultureService;
using SmartWalk.Server.Services.EntityService;
using SmartWalk.Server.ViewModels;

namespace SmartWalk.Server.Services.EventService
{
    public class EventService : IEventService {
        private readonly IEntityService _entityService;
        private readonly IRepository<EventMetadataRecord> _eventMetadataRepository;

        public EventService(IEntityService entityService, IRepository<EventMetadataRecord> eventMetadataRepository)
        {
            _entityService = entityService;
            _eventMetadataRepository = eventMetadataRepository;
        }

        public IList<EventMetadataVm> GetUserEvents(SmartWalkUserRecord user) {
            return user.EventMetadataRecords.Select(CreateViewModelContract).ToList();
        }

        public EventMetadataFullVm GetUserEventVmById(SmartWalkUserRecord user, int id) {
            var eventMetadata = user.EventMetadataRecords.FirstOrDefault(u => u.Id == id);
            if (eventMetadata != null) {
                return new EventMetadataFullVm {
                    EventMetadata = CreateViewModelContract(eventMetadata),
                    Hosts = _entityService.GetUserEntities(user, EntityType.Host),                    
                    Venues = _entityService.GetUserEntities(user, EntityType.Venue)
                };
            }

            return null;
        }        

        private EventMetadataVm CreateViewModelContract(EventMetadataRecord record)
        {
            if (record == null)
                return null;

            var res = ViewModelContractFactory.CreateViewModelContract(record);

            res.Host = _entityService.GetEntityVm(record.EntityRecord);
            res.AllVenues = _entityService.GetEventEntities(record);

            return res;
        }

        public void DeleteEvent(EventMetadataVm item) {
            foreach (var venue in item.AllVenues)
            {
                _entityService.DeleteEventVenue(venue);
            }

            var metadata = _eventMetadataRepository.Get(item.Id);

            if (metadata == null)
                return;

            _eventMetadataRepository.Delete(metadata);
            _eventMetadataRepository.Flush();
        }
    }
}