using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using Orchard.Data;
using SmartWalk.Server.Records;
using SmartWalk.Server.ViewModels;

namespace SmartWalk.Server.Services.EventService
{
    public class EventService : IEventService
    {
        private readonly IRepository<RegionRecord> _regionRepository;

        public EventService(IRepository<RegionRecord> regionRepository) {
            _regionRepository = regionRepository;
        }

        public IList<EventMetadataVm> GetUserEvents(SmartWalkUserRecord user) {
            return user.EventMetadataRecords.Select(ViewModelContractFactory.CreateViewModelContract).ToList();
        }

        public EventMetadataFullVm GetUserEventVmById(SmartWalkUserRecord user, int id) {
            return new EventMetadataFullVm {
                EventMetadata = ViewModelContractFactory.CreateViewModelContract(user.EventMetadataRecords.FirstOrDefault(u => u.Id == id)),
                Hosts = user.Entities.Where(e => e.Type == (int)EntityType.Host).Select(ViewModelContractFactory.CreateViewModelContract).ToList(),
                Regions = _regionRepository.Table.Select(ViewModelContractFactory.CreateViewModelContract).ToList()
            };
        }        
    }
}