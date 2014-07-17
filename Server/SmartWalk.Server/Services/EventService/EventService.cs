using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using Orchard.Data;
using SmartWalk.Server.Records;
using SmartWalk.Server.Services.EntityService;
using SmartWalk.Server.Utils;
using SmartWalk.Server.ViewModels;
using SmartWalk.Shared;

namespace SmartWalk.Server.Services.EventService
{
    [UsedImplicitly]
    public class EventService : IEventService {
        private readonly IEntityService _entityService;
        private readonly IRepository<EventMetadataRecord> _eventMetadataRepository;
        private readonly IRepository<EntityRecord> _entityRepository;
        private readonly IRepository<ShowRecord> _showRepository;

        public EventService(IEntityService entityService, 
            IRepository<EventMetadataRecord> eventMetadataRepository,
            IRepository<EntityRecord> entityRepository,
            IRepository<ShowRecord> showRepository)
        {
            _entityService = entityService;
            _eventMetadataRepository = eventMetadataRepository;
            _entityRepository = entityRepository;
            _showRepository = showRepository;
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

        public IList<EventMetadataVm> GetEvents(
            SmartWalkUserRecord user,
            int pageNumber,
            int pageSize,
            Func<EventMetadataRecord, IComparable> orderBy,
            bool isDesc = false,
            string searchString = null)
        {
            return GetEventsInner(
                user == null
                    ? (IEnumerable<EventMetadataRecord>)
                      _eventMetadataRepository.Table.Where(e => e.IsPublic)
                    : user.EventMetadataRecords, pageNumber, pageSize, orderBy, isDesc, searchString);
        }

        private IList<EventMetadataVm> GetEventsInner(IEnumerable<EventMetadataRecord> query, int pageNumber, int pageSize, Func<EventMetadataRecord, IComparable> orderBy, bool isDesc, string searchString)
        {
            query = query.Where(e => !e.IsDeleted);
            if(!string.IsNullOrEmpty(searchString))
                query = query.Where(e => !string.IsNullOrEmpty(e.Title) && e.Title.ToLower(CultureInfo.InvariantCulture).Contains(searchString.ToLower(CultureInfo.InvariantCulture)));
            query = isDesc ? query.OrderByDescending(orderBy) : query.OrderBy(orderBy);
            return query.Skip(pageSize * pageNumber).Take(pageSize).Select(e => CreateViewModelContract(e, LoadMode.Compact)).ToList();
        }

        public EventMetadataVm GetEventVmById(int id) {
            var eventMetadata = _eventMetadataRepository.Table.FirstOrDefault(u => u.Id == id);

            var vm = CreateViewModelContract(eventMetadata);

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

        private EventMetadataVm CreateViewModelContract(EventMetadataRecord record) {
            if (record != null)
                return CreateViewModelContract(record, LoadMode.Full);

            var res = new EventMetadataVm {
                Id = 0,
            };

            return res;
        }

        private EventMetadataVm CreateViewModelContract(EventMetadataRecord record, LoadMode mode) {
            var res = ViewModelContractFactory.CreateViewModelContract(record, mode);
            res.Host = _entityService.GetEntityVm(record.EntityRecord, LoadMode.Compact);

            if (mode == LoadMode.Full) {
                res.Venues = _entityService.GetEventEntities(record);
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


        private void RecalculateEventCoordinates(EventMetadataRecord eventRecord)
        {
            var coords = eventRecord
                .ShowRecords
                .Where(s => !s.IsDeleted)
                .Select(s => s.EntityRecord)
                .Where(v => !v.IsDeleted)
                .SelectMany(v => v.AddressRecords)
                .Select(address => new PointF((float)address.Latitude, (float)address.Longitude))
                .ToList();

            if (coords.Count == 0) return;

            var eventCoord = MapUtil.GetMiddleCoordinate(coords.ToArray());

            eventRecord.Latitude = eventCoord.X;
            eventRecord.Longitude = eventCoord.Y;

            _eventMetadataRepository.Flush();
        }

        public EventMetadataVm SaveOrAddEvent(SmartWalkUserRecord user, EventMetadataVm item) {
            var host = _entityRepository.Get(item.Host.Id);

            if (host == null || item.StartDate == null) return null;            

            var metadata =_eventMetadataRepository.Get(item.Id);

            if (metadata == null)
            {
                metadata = new EventMetadataRecord
                {
                    EntityRecord = host,
                    SmartWalkUserRecord = user,
                    Title = item.Title,
                    Description = item.Description,
                    StartTime = item.StartDate.Value,
                    EndTime = item.EndDate,
                    CombineType = item.CombineType,
                    Picture = item.Picture,
                    IsPublic = item.IsPublic,
                    IsDeleted = false,
                    DateCreated = DateTime.UtcNow,
                    DateModified = DateTime.UtcNow,
                };

                _eventMetadataRepository.Create(metadata);
            }
            else {
                metadata.EntityRecord = host;
                metadata.Title = item.Title;
                metadata.Description = item.Description;
                metadata.StartTime = item.StartDate.Value;
                metadata.EndTime = item.EndDate;
                metadata.CombineType = item.CombineType;
                metadata.Picture = item.Picture;
                metadata.IsPublic = item.IsPublic;
                metadata.DateModified = DateTime.UtcNow;                
            }

            _eventMetadataRepository.Flush();

            foreach (var venueVm in item.Venues.Where(venueVm => !venueVm.Destroy)) {
                var currentVenue = venueVm;

                if (venueVm.Id < 0) {
                    currentVenue = _entityService.SaveOrAddEntity(user, venueVm);

                    if (currentVenue == null)
                        continue;
                }

                if (!venueVm.Shows.Any())
                    _entityService.CheckShowVenue(metadata.Id, currentVenue.Id);
                else {
                    foreach (var showVm in venueVm.Shows.Where(showVm => !showVm.Destroy)) {
                        _entityService.SaveOrAddShow(showVm, metadata.Id, currentVenue.Id);
                    }
                }
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