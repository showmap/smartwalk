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
using SmartWalk.Shared.Utils;

namespace SmartWalk.Server.Services.EventService
{
    [UsedImplicitly]
    public class EventService : IEventService
    {
        private readonly IEntityService _entityService; // TODO: We should get rid of this reference
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

        public IList<EventMetadataVm> GetEvents(
            SmartWalkUserRecord user,
            int pageNumber,
            int pageSize,
            Func<EventMetadataRecord, IComparable> orderBy,
            bool isDesc = false,
            string searchString = null)
        {
            var query =
                user == null
                    ? (IEnumerable<EventMetadataRecord>)_eventMetadataRepository
                        .Table
                        .Where(e => e.IsPublic)
                    : user.EventMetadataRecords;

            var result = GetEvents(query, pageNumber, pageSize, orderBy, isDesc, searchString);
            return result;
        }

        public IList<EventMetadataVm> GetEventsByEntity(int entityId)
        {
            var entity = _entityRepository.Get(entityId);
            if (entity == null) return new List<EventMetadataVm>();

            IEnumerable<EventMetadataRecord> query;

            if ((EntityType)entity.Type == EntityType.Venue)
            {
                query = _eventMetadataRepository
                    .Table
                    .Where(md => md.ShowRecords.Any(s => s.EntityRecord.Id == entityId));
            }
            else
            {
                query = entity.EventMetadataRecords;
            }

            var result = GetEvents(query, 0, 5, e => e.StartTime, true);
            return result;
        }

        public EventMetadataVm GetEventById(int id, int? day)
        {
            if (day != null && day < 0) throw new ArgumentOutOfRangeException("day");

            var eventMeta = _eventMetadataRepository.Get(id);
            if (eventMeta == null) return null;

            var result = CreateViewModelContract(eventMeta, day);
            return result;
        }

        public AccessType GetEventAccess(SmartWalkUserRecord user, int eventId)
        {
            if (user == null) return AccessType.Deny;

            var eventMeta = _eventMetadataRepository.Get(eventId);
            if (eventMeta == null || eventMeta.IsDeleted) return AccessType.Deny;
            if (eventMeta.SmartWalkUserRecord.Id == user.Id) return AccessType.AllowEdit;

            return eventMeta.IsPublic ? AccessType.AllowView : AccessType.Deny;
        }

        public EventMetadataVm SaveEvent(SmartWalkUserRecord user, EventMetadataVm eventVm)
        {
            var host = _entityRepository.Get(eventVm.Host.Id);
            if (host == null || eventVm.StartDate == null) return null;

            var eventMeta = _eventMetadataRepository.Get(eventVm.Id);
            if (eventMeta == null)
            {
                eventMeta = new EventMetadataRecord
                    {
                        EntityRecord = host,
                        SmartWalkUserRecord = user,
                        Title = eventVm.Title,
                        Description = eventVm.Description,
                        StartTime = eventVm.StartDate.Value,
                        EndTime = eventVm.EndDate,
                        CombineType = eventVm.CombineType,
                        Picture = eventVm.Picture,
                        IsPublic = eventVm.IsPublic,
                        IsDeleted = false,
                        DateCreated = DateTime.UtcNow,
                        DateModified = DateTime.UtcNow,
                    };

                _eventMetadataRepository.Create(eventMeta);
            }
            else
            {
                eventMeta.EntityRecord = host;
                eventMeta.Title = eventVm.Title;
                eventMeta.Description = eventVm.Description;
                eventMeta.StartTime = eventVm.StartDate.Value;
                eventMeta.EndTime = eventVm.EndDate;
                eventMeta.CombineType = eventVm.CombineType;
                eventMeta.Picture = eventVm.Picture;
                eventMeta.IsPublic = eventVm.IsPublic;
                eventMeta.DateModified = DateTime.UtcNow;
            }

            _eventMetadataRepository.Flush();

            foreach (var venue in eventVm.Venues.Where(venueVm => !venueVm.Destroy))
            {
                var currentVenue = venue;

                if (venue.Id < 0)
                {
                    // TODO: We are not supposed to save entities on event save
                    // this is probably a mistake.
                    currentVenue = _entityService.SaveEntity(user, venue);
                    if (currentVenue == null) continue;
                }

                if (!venue.Shows.Any())
                {
                    CheckShowVenue(eventMeta.Id, currentVenue.Id);
                }
                else
                {
                    foreach (var showVm in venue.Shows)
                    {
                        SaveShow(showVm, eventMeta.Id, currentVenue.Id);
                    }
                }
            }

            RecalculateEventCoordinates(eventMeta);

            return CreateViewModelContract(eventMeta, null);
        }

        public void DeleteEvent(int eventId)
        {
            var eventMeta = _eventMetadataRepository.Get(eventId);
            if (eventMeta == null) return;

            foreach (var show in eventMeta.ShowRecords)
            {
                show.IsDeleted = true;
                _showRepository.Flush();
            }

            eventMeta.IsDeleted = true;
            _eventMetadataRepository.Flush();
        }

        private static IList<EventMetadataVm> GetEvents(
            IEnumerable<EventMetadataRecord> query, 
            int pageNumber,
            int pageSize,
            Func<EventMetadataRecord, IComparable> orderBy,
            bool isDesc = false,
            string searchString = null)
        {
            query = query.Where(e => !e.IsDeleted);

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                query =
                    query.Where(
                        e =>
                        !string.IsNullOrEmpty(e.Title) &&
                        e.Title.ToLower(CultureInfo.InvariantCulture)
                         .Contains(searchString.ToLower(CultureInfo.InvariantCulture)));
            }

            query = isDesc ? query.OrderByDescending(orderBy) : query.OrderBy(orderBy);

            var result =
                query.Skip(pageSize * pageNumber)
                     .Take(pageSize)
                     .Select(e => CreateViewModelContract(e, null, LoadMode.Compact))
                     .ToList();
            return result;
        }

        private static EventMetadataVm CreateViewModelContract(
            EventMetadataRecord eventMeta,
            int? day,
            LoadMode mode = LoadMode.Full)
        {
            if (eventMeta == null) throw new ArgumentNullException("eventMeta");

            var result = ViewModelContractFactory.CreateViewModelContract(eventMeta, mode);
            result.Host =
                EntityService
                    .ViewModelContractFactory
                    .CreateViewModelContract(eventMeta.EntityRecord, mode);

            if (mode == LoadMode.Full)
            {
                result.Venues = GetEventVenues(eventMeta, day);
            }

            return result;
        }

        private static IList<EntityVm> GetEventVenues(EventMetadataRecord eventMeta, int? day)
        {
            var range = new Tuple<DateTime, DateTime?>(eventMeta.StartTime, eventMeta.EndTime);
            var currentDay =
                day == null
                    ? null
                    : (eventMeta.IsMultiDay()
                           ? (DateTime?)eventMeta.StartTime.AddDays(day.Value)
                           : null);

            var allShows =
                eventMeta.ShowRecords
                        .Where(s =>
                               !s.IsDeleted &&
                               (s.IsReference || s.StartTime.IsTimeThisDay(currentDay, range)))
                        .ToArray();

            var result = allShows
                .Select(s => s.EntityRecord)
                .Distinct()
                .Select(e =>
                    {
                        var venueVm = EntityService
                            .ViewModelContractFactory
                            .CreateViewModelContract(e, LoadMode.Full);

                        venueVm.Shows = allShows
                            .Where(s => s.EntityRecord.Id == e.Id && !s.IsReference)
                            .Select(ViewModelContractFactory.CreateViewModelContract)
                            .ToArray();

                        return venueVm;
                    })
                .ToArray();
            return result;
        }

        private void CheckShowVenue(int eventId, int venueId)
        {
            var shows =
                _showRepository
                    .Table
                    .Where(
                        s =>
                        s.EventMetadataRecord.Id == eventId &&
                        s.EntityRecord.Id == venueId &&
                        (!s.IsDeleted || s.IsReference))
                    .ToList();

            if (shows.Any())
            {
                // If we have both is reference and not is reference shows, remove all is reference shows
                if (shows.Count(s => !s.IsReference) > 0 && shows.Count(s => s.IsReference) > 0)
                {
                    foreach (var showRecord in shows.Where(s => s.IsReference))
                    {
                        _showRepository.Delete(showRecord);
                        _showRepository.Flush();
                    }
                }
            }
            else
            {
                var show = new ShowRecord
                    {
                        EventMetadataRecord = _eventMetadataRepository.Get(eventId),
                        EntityRecord = _entityRepository.Get(venueId),
                        IsReference = true,
                        IsDeleted = false,
                        DateCreated = DateTime.UtcNow,
                        DateModified = DateTime.UtcNow
                    };

                _showRepository.Create(show);
                _showRepository.Flush();
            }
        }

        private void SaveShow(ShowVm item, int eventId, int venueId)
        {
            var show = _showRepository.Get(item.Id);
            var eventMeta = _eventMetadataRepository.Get(eventId);
            var venue = _entityRepository.Get(venueId);

            if (eventMeta == null || venue == null) return;

            if (show == null) {

                if (item.Destroy)
                    return;

                show = new ShowRecord
                    {
                        EventMetadataRecord = eventMeta,
                        EntityRecord = venue,
                        IsReference = false,
                        Title = item.Title,
                        Description = item.Description,
                        StartTime = item.StartTime,
                        EndTime = item.EndTime,
                        Picture = item.Picture,
                        DetailsUrl = item.DetailsUrl,
                        IsDeleted = false,
                        DateCreated = DateTime.UtcNow,
                        DateModified = DateTime.UtcNow
                    };

                _showRepository.Create(show);
                _showRepository.Flush();

                CheckShowVenue(eventMeta.Id, venue.Id);
            }
            else {
                if (item.Destroy)
                    show.IsDeleted = true;

                show.EntityRecord = venue;
                show.Title = item.Title;
                show.Description = item.Description;
                show.StartTime = item.StartTime;
                show.EndTime = item.EndTime;
                show.Picture = item.Picture;
                show.DetailsUrl = item.DetailsUrl;
                show.DateModified = DateTime.UtcNow;

                _showRepository.Flush();
            }
        }

        private void RecalculateEventCoordinates(EventMetadataRecord eventMeta)
        {
            var coords = eventMeta
                .ShowRecords
                .Where(s => !s.IsDeleted)
                .Select(s => s.EntityRecord)
                .Where(v => !v.IsDeleted)
                .SelectMany(v => v.AddressRecords)
                .Select(address => new PointF((float)address.Latitude, (float)address.Longitude))
                .ToArray();

            if (coords.Length == 0) return;

            var eventCoord = MapUtil.GetMiddleCoordinate(coords);

            eventMeta.Latitude = eventCoord.X;
            eventMeta.Longitude = eventCoord.Y;

            _eventMetadataRepository.Flush();
        }
    }
}