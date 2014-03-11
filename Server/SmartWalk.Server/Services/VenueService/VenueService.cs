using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Data;
using SmartWalk.Server.Records;
using SmartWalk.Server.ViewModels;

namespace SmartWalk.Server.Services.VenueService
{
    public class VenueService : IVenueService
    {
        private readonly IRepository<EntityRecord> _entityRepository;

        public VenueService(IRepository<EntityRecord> entityRepository) {
            _entityRepository = entityRepository;
        }

        public IList<EntityVm> GetUserVenues(SmartWalkUserRecord user) {
            return _entityRepository.Table.Where(e => e.Type == (int) EntityType.Venue && (e.SmartWalkUserRecord.Id == user.Id || e.ShowRecords.Any(s => s.EntityRecord.SmartWalkUserRecord.Id == user.Id))).Select(ViewModelContractFactory.CreateViewModelContract).ToList();
        }

    }
}