using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using Orchard.Data;
using SmartWalk.Server.Records;
using SmartWalk.Server.Services.CultureService;
using SmartWalk.Server.Services.EntityService;
using SmartWalk.Server.Utils;
using SmartWalk.Server.ViewModels;
using SmartWalk.Shared.Utils;

namespace SmartWalk.Server.Services.EventService
{
    public class EventService : IEventService {
        private readonly IEntityService _entityService;
        private readonly IRepository<EventMetadataRecord> _eventMetadataRepository;
        private readonly IRepository<EntityRecord> _entityRepository;
        private readonly IRepository<SmartWalkUserRecord> _userRepository;
        private readonly IRepository<ShowRecord> _showRepository;

        private readonly Lazy<CultureInfo> _cultureInfo;

        public EventService(ICultureService cultureService, IEntityService entityService, 
            IRepository<EventMetadataRecord> eventMetadataRepository, IRepository<EntityRecord> entityRepository,
            IRepository<SmartWalkUserRecord> userRepository, IRepository<ShowRecord> showRepository)
        {
            _entityService = entityService;
            _eventMetadataRepository = eventMetadataRepository;
            _entityRepository = entityRepository;
            _showRepository = showRepository;
            _userRepository = userRepository;

            _cultureInfo = new Lazy<CultureInfo>(cultureService.GetCurrentCulture);
        }

        public IList<EventMetadataVm> GetEntityEvents(int entityId) {
            var entity = _entityRepository.Get(entityId);

            if(entity == null)
                return new List<EventMetadataVm>();

            IEnumerable<EventMetadataRecord> data = entity.EventMetadataRecords;

            if ((EntityType)entity.Type == EntityType.Venue)
                data = _eventMetadataRepository.Table.Where(md => md.ShowRecords.Any(s => s.EntityRecord.Id == entityId));

            return data.Where(e => !e.IsDeleted).OrderByDescending(e => e.StartTime).Take(5).Select(e => CreateViewModelContract(e, LoadMode.Compact)).ToList();
        }

        public IList<EventMetadataVm> GetEvents(SmartWalkUserRecord user, int pageNumber, int pageSize, Func<EventMetadataRecord, IComparable> orderBy, bool isDesc, string searchString) {
            return GetEventsInner(user == null ? (IEnumerable<EventMetadataRecord>) _eventMetadataRepository.Table.Where(e => e.IsPublic) : user.EventMetadataRecords, pageNumber, pageSize, orderBy, isDesc, searchString);
        }

        private IList<EventMetadataVm> GetEventsInner(IEnumerable<EventMetadataRecord> query, int pageNumber, int pageSize, Func<EventMetadataRecord, IComparable> orderBy, bool isDesc, string searchString)
        {
            query = query.Where(e => !e.IsDeleted);
            if(!string.IsNullOrEmpty(searchString))
                query = query.Where(e => !string.IsNullOrEmpty(e.Title) && e.Title.ToLower(CultureInfo.InvariantCulture).Contains(searchString.ToLower(CultureInfo.InvariantCulture)));
            query = isDesc ? query.OrderByDescending(orderBy) : query.OrderBy(orderBy);
            return query.Skip(pageSize * pageNumber).Take(pageSize).Select(e => CreateViewModelContract(e, LoadMode.Compact)).ToList();
        }

        public EventMetadataVm GetUserEventVmById(SmartWalkUserRecord user, int id) {
            var eventMetadata = user.EventMetadataRecords.FirstOrDefault(u => u.Id == id);

            var vm = CreateViewModelContract(user, eventMetadata);
            //vm.AllHosts = _entityService.GetUserEntities(user, EntityType.Host, 0, 8, e => e.Name, false, "");

            return vm;
        }

        public AccessType GetEventAccess(SmartWalkUserRecord user, int eventId) {
            if (user == null)
                return AccessType.Deny;

            if (eventId == 0)
                return AccessType.AllowEdit;

            var eventRecord = _eventMetadataRepository.Get(eventId);
            if (eventRecord == null || eventRecord.IsDeleted)
                return AccessType.Deny;

            if(eventRecord.SmartWalkUserRecord.Id == user.Id)
                return AccessType.AllowEdit;

            return eventRecord.IsPublic ? AccessType.AllowView : AccessType.Deny;
        }

        private EventMetadataVm CreateViewModelContract(SmartWalkUserRecord user, EventMetadataRecord record) {
            if (record != null)
                return CreateViewModelContract(record);

            var res = new EventMetadataVm {
                Id = 0,
            };

            return res;
        }

        private EventMetadataVm CreateViewModelContract(EventMetadataRecord record, LoadMode mode = LoadMode.Full) {
            var res = ViewModelContractFactory.CreateViewModelContract(record, mode);
            res.Host = _entityService.GetEntityVm(record.EntityRecord, LoadMode.Compact);

            if (mode == LoadMode.Full) {
                res.AllVenues = _entityService.GetEventEntities(record);
            }

            return res;
        }

        public void DeleteEvent(int eventId) {
            var metadata = _eventMetadataRepository.Get(eventId);

            if (metadata == null)
                return;

            foreach (var show in metadata.ShowRecords) {
                show.IsDeleted = true;
                _showRepository.Flush();
            }

            metadata.IsDeleted = true;
            _eventMetadataRepository.Flush();
        }


        public void RecalculateEventCoordinates(EventMetadataRecord eventRecord) {
            var coords = eventRecord.ShowRecords
                                    .Where(s => !s.IsDeleted)
                                    .Select(s => s.EntityRecord)
                                    .Where(v => !v.IsDeleted)
                                    .SelectMany(v => v.AddressRecords)
                                    .Select(address => new PointF((float) address.Latitude, (float) address.Longitude)).ToList();

            if (coords.Count == 0)
                return;

            var eventCoord = MapUtil.GetMiddleCoordinate(coords.ToArray());

            eventRecord.Latitude = eventCoord.X;
            eventRecord.Longitude = eventCoord.Y;

            _eventMetadataRepository.Flush();
        }

        public EventMetadataVm SaveOrAddEvent(SmartWalkUserRecord user, EventMetadataVm item) {
            var host = _entityRepository.Get(item.Host.Id);
            var dtFrom = item.StartTime.ParseDateTime(_cultureInfo.Value);

            if (host == null || dtFrom == null)
                return null;            

            var metadata =_eventMetadataRepository.Get(item.Id);
            var dtTo = item.EndTime.ParseDateTime(_cultureInfo.Value);

            if (metadata == null)
            {
                metadata = new EventMetadataRecord
                {
                    EntityRecord = host,
                    SmartWalkUserRecord = user,
                    Title = item.Title,
                    Description = item.Description,
                    StartTime = dtFrom.Value,
                    EndTime = dtTo,
                    CombineType = item.CombineType,
                    Picture = item.Picture,
                    IsPublic = item.IsPublic,
                    IsDeleted = false,
                    DateCreated = DateTime.Now,
                    DateModified = DateTime.Now,
                };

                _eventMetadataRepository.Create(metadata);
            }
            else {
                metadata.EntityRecord = host;
                metadata.Title = item.Title;
                metadata.Description = item.Description;
                metadata.StartTime = dtFrom.Value;
                metadata.EndTime = dtTo;
                metadata.CombineType = item.CombineType;
                metadata.Picture = item.Picture;
                metadata.IsPublic = item.IsPublic;
                metadata.DateModified = DateTime.Now;                
            }

            _eventMetadataRepository.Flush();

            foreach (var showVm in item.AllVenues.Where(venueVm => venueVm.State != VmItemState.Deleted).SelectMany(venueVm => venueVm.AllShows.Where(showVm => showVm.State != VmItemState.Deleted))) {
                showVm.EventMetadataId = metadata.Id;
                _entityService.SaveOrAddShow(showVm);
            }

            RecalculateEventCoordinates(metadata);

            return CreateViewModelContract(metadata);
        }
    }

    public enum LoadMode {
        Full,
        Compact
    }
}