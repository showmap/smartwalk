using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using Orchard.Data;
using SmartWalk.Server.Records;
using SmartWalk.Server.Utils;
using SmartWalk.Server.ViewModels;
using SmartWalk.Shared;
using SmartWalk.Shared.Utils;

namespace SmartWalk.Server.Services.EventService
{
    [UsedImplicitly]
    public class EventService : IEventService
    {
        private readonly IRepository<EventMetadataRecord> _eventMetadataRepository;
        private readonly IRepository<EntityRecord> _entityRepository;

        public EventService(
            IRepository<EventMetadataRecord> eventMetadataRepository,
            IRepository<EntityRecord> entityRepository)
        {
            _eventMetadataRepository = eventMetadataRepository;
            _entityRepository = entityRepository;
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
            if (user == null) throw new ArgumentNullException("user");
            if (eventVm == null) throw new ArgumentNullException("eventVm");
            if (eventVm.StartDate == null) throw new ArgumentNullException("eventVm.StartDate");

            var host = _entityRepository.Get(eventVm.Host.Id);
            if (host == null) throw new ArgumentOutOfRangeException("eventVm.Host");
            if (host.SmartWalkUserRecord.Id != user.Id) 
                throw new ArgumentOutOfRangeException("eventVm.Host", "Can't add host created by other user to the event.");

            var eventMeta = _eventMetadataRepository.Get(eventVm.Id) ?? new EventMetadataRecord
                {
                    SmartWalkUserRecord = user,
                    DateCreated = DateTime.UtcNow
                };

            ViewModelFactory.UpdateByViewModel(eventMeta, eventVm, host);

            foreach (var venueVm in eventVm.Venues)
            {
                var venue = _entityRepository.Get(venueVm.Id);

                foreach (var showVm in venueVm.Shows)
                {
                    // ReSharper disable AccessToForEachVariableInClosure
                    var show = eventMeta.ShowRecords.FirstOrDefault(s => s.Id == showVm.Id);
                    // ReSharper restore AccessToForEachVariableInClosure
                    if (show == null)
                    {
                        show = new ShowRecord
                            {
                                EventMetadataRecord = eventMeta,
                                EntityRecord = venue,
                                DateCreated = DateTime.UtcNow
                            };
                        eventMeta.ShowRecords.Add(show);
                    }

                    // if parent venue is deleted, all shows are deleted too
                    if (venueVm.Destroy)
                    {
                        showVm.Destroy = true;
                    }

                    ViewModelFactory.UpdateByViewModel(show, showVm);
                }

                UpdateReferenceShow(eventMeta, venue, 
                    !venueVm.Destroy && venueVm.Shows.Count(s => !s.Destroy) == 0);
            }

            RecalcEventCoordinates(eventMeta);

            if (eventMeta.Id > 0)
            {
                _eventMetadataRepository.Update(eventMeta);
            }
            else
            {
                _eventMetadataRepository.Create(eventMeta);
            }

            _eventMetadataRepository.Flush();

            var result = CreateViewModelContract(eventMeta, null);
            return result;
        }

        public void DeleteEvent(int eventId)
        {
            var eventMeta = _eventMetadataRepository.Get(eventId);
            if (eventMeta == null) return;

            foreach (var show in eventMeta.ShowRecords.Where(s => !s.IsDeleted).ToArray())
            {
                show.IsDeleted = true;
                show.DateModified = DateTime.UtcNow;
            }

            eventMeta.IsDeleted = true;
            eventMeta.DateModified = DateTime.UtcNow;

            _eventMetadataRepository.Update(eventMeta);
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
                        // ReSharper disable PossibleNullReferenceException
                         .Contains(searchString.ToLower(CultureInfo.InvariantCulture)));
                        // ReSharper restore PossibleNullReferenceException
            }

            query = isDesc ? query.OrderByDescending(orderBy) : query.OrderBy(orderBy);

            var result =
                query.Skip(pageSize * pageNumber)
                     .Take(pageSize)
                     .Select(e => CreateViewModelContract(e, null, LoadMode.Compact))
                     .ToArray();
            return result;
        }

        private static EventMetadataVm CreateViewModelContract(
            EventMetadataRecord eventMeta,
            int? day,
            LoadMode mode = LoadMode.Full)
        {
            if (eventMeta == null) throw new ArgumentNullException("eventMeta");

            var result = ViewModelFactory.CreateViewModel(eventMeta, mode);
            result.Host =
                EntityService
                    .ViewModelFactory
                    .CreateViewModel(eventMeta.EntityRecord, mode);

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
                            .ViewModelFactory
                            .CreateViewModel(e, LoadMode.Full);

                        venueVm.Shows = allShows
                            .Where(s => s.EntityRecord.Id == e.Id && !s.IsReference)
                            .Select(ViewModelFactory.CreateViewModel)
                            .ToArray();

                        return venueVm;
                    })
                .ToArray();
            return result;
        }

        private static void UpdateReferenceShow(
            EventMetadataRecord eventMeta,
            EntityRecord venue,
            bool addReference)
        {
            var refShow = eventMeta
                .ShowRecords
                .FirstOrDefault(
                    s =>
                    s.EntityRecord.Id == venue.Id &&
                    s.IsReference);

            if (addReference)
            {
                // creating a new ref show if venue is empty and there was no ref before
                if (refShow == null)
                {
                    refShow = new ShowRecord
                        {
                            EventMetadataRecord = eventMeta,
                            EntityRecord = venue,
                            DateCreated = DateTime.UtcNow,
                            IsReference = true
                        };
                    eventMeta.ShowRecords.Add(refShow);
                }
                // restoring existing ref show
                else
                {
                    refShow.IsDeleted = false;
                }
            }
            else
            {
                // if ref show exists mark it as deleted
                if (refShow != null)
                {
                    refShow.IsDeleted = true;
                }
            }

            if (refShow != null)
            {
                refShow.DateModified = DateTime.UtcNow;
            }
        }

        private static void RecalcEventCoordinates(EventMetadataRecord eventMeta)
        {
            var coords = eventMeta
                .ShowRecords
                .Where(s => !s.IsDeleted)
                .Select(s => s.EntityRecord)
                .SelectMany(v => v.AddressRecords)
                .Select(address => new PointF((float)address.Latitude, (float)address.Longitude))
                .ToArray();

            if (coords.Length != 0)
            {
                var eventCoord = MapUtil.GetMiddleCoordinate(coords);
                eventMeta.Latitude = eventCoord.X;
                eventMeta.Longitude = eventCoord.Y;
            }
            else
            {
                eventMeta.Latitude = 0;
                eventMeta.Longitude = 0;
            }
        }
    }
}