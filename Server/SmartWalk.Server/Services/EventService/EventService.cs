using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Security;
using Orchard;
using Orchard.Data;
using SmartWalk.Server.Records;
using SmartWalk.Server.Services.Base;
using SmartWalk.Server.Utils;
using SmartWalk.Server.ViewModels;
using SmartWalk.Shared;
using SmartWalk.Shared.Utils;

namespace SmartWalk.Server.Services.EventService
{
    [UsedImplicitly]
    public class EventService : OrchardBaseService, IEventService
    {
        private readonly IRepository<EventMetadataRecord> _eventMetadataRepository;
        private readonly IRepository<EntityRecord> _entityRepository;

        public EventService(
            IOrchardServices orchardServices,
            IRepository<EventMetadataRecord> eventMetadataRepository,
            IRepository<EntityRecord> entityRepository)
            : base(orchardServices)
        {
            _eventMetadataRepository = eventMetadataRepository;
            _entityRepository = entityRepository;
        }

        public AccessType GetEventsAccess()
        {
            var result = SecurityUtils.GetAccess(Services.Authorizer);
            return result;
        }

        public AccessType GetEventAccess(int eventId)
        {
            var eventMeta = _eventMetadataRepository.Get(eventId);
            var access = eventMeta.GetAccess(Services.Authorizer, CurrentUser);
            var result =
                eventMeta.Status == (byte)EventStatus.Private
                && access != AccessType.AllowEdit
                    ? AccessType.Deny
                    : access;
            return result;
        }

        public IList<EventMetadataVm> GetEvents(
            DisplayType display,
            int pageNumber,
            int pageSize,
            Func<EventMetadataRecord, IComparable> orderBy,
            bool isDesc = false,
            string searchString = null)
        {
            var access = GetEventsAccess();
            if (access == AccessType.Deny)
                throw new SecurityException("Can't get events.");

            if (display == DisplayType.My && CurrentUser == null)
                throw new SecurityException("Can't show my events without user.");

            var query =
                display == DisplayType.All
                    ? (IEnumerable<EventMetadataRecord>)_eventMetadataRepository
                        .Table
                        .Where(e => e.Status == (byte)EventStatus.Public)
                    : CurrentUser.EventMetadataRecords;

            var result = GetEvents(query, pageNumber, pageSize, orderBy, isDesc, searchString);
            return result;
        }

        public IList<EventMetadataVm> GetEventsByEntity(int entityId)
        {
            var access = GetEventsAccess();
            if (access == AccessType.Deny)
                throw new SecurityException("Can't get events.");

            var entity = _entityRepository.Get(entityId);
            if (entity == null || entity.IsDeleted) return null;

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

        /// <summary>
        /// Gets an event by id and filters shows by day.
        /// </summary>
        /// <param name="id">The id of the event.</param>
        /// <param name="day">The day to filter shows by. If Null all shows are returned. 
        /// The day values start from 0 (day 1) and up.</param>
        public EventMetadataVm GetEventById(int id, int? day = null)
        {
            if (day != null && day < 0) throw new ArgumentOutOfRangeException("day");

            var eventMeta = _eventMetadataRepository.Get(id);
            if (eventMeta == null || eventMeta.IsDeleted) return null;

            var access = eventMeta.GetAccess(Services.Authorizer, CurrentUser);
            if (access == AccessType.Deny)
                throw new SecurityException("Can't get event.");

            var result = CreateViewModelContract(eventMeta, day);
            return result;
        }

        public EventMetadataVm SaveEvent(EventMetadataVm eventVm)
        {
            if (eventVm == null) throw new ArgumentNullException("eventVm");
            if (eventVm.StartDate == null) throw new ArgumentNullException("eventVm.StartDate");

            var host = _entityRepository.Get(eventVm.Host.Id);
            if (host == null) throw new ArgumentOutOfRangeException("eventVm.Host");
            if (!Services.Authorizer.Authorize(Permissions.UseAllContent) 
                    && host.SmartWalkUserRecord.Id != CurrentUser.Id) 
                throw new ArgumentOutOfRangeException("eventVm.Host", "Can't add host created by other user to the event.");

            var eventMeta = _eventMetadataRepository.Get(eventVm.Id) ?? new EventMetadataRecord
                {
                    SmartWalkUserRecord = CurrentUser,
                    DateCreated = DateTime.UtcNow
                };

            var access = eventMeta.GetAccess(Services.Authorizer, CurrentUser);
            if (access != AccessType.AllowEdit)
                throw new SecurityException("Can't edit event.");

            if (eventMeta.IsDeleted)
                throw new InvalidOperationException("Can't edit deleted event.");

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

            if (eventMeta.Id == 0)
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
            if (eventMeta == null || eventMeta.IsDeleted) return;

            var access = eventMeta.GetAccess(Services.Authorizer, CurrentUser);
            if (access != AccessType.AllowEdit)
                throw new SecurityException("Can't delete event.");

            foreach (var show in eventMeta.ShowRecords.Where(s => !s.IsDeleted).ToArray())
            {
                show.IsDeleted = true;
                show.DateModified = DateTime.UtcNow;
            }

            eventMeta.IsDeleted = true;
            eventMeta.DateModified = DateTime.UtcNow;

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
                    .ViewModelFactory.CreateViewModel(eventMeta.EntityRecord, mode);

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
                .OrderBy(e => e.Name)
                .Select(e =>
                    {
                        var venueVm = EntityService
                            .ViewModelFactory
                            .CreateViewModel(e, LoadMode.Full);

                        venueVm.Shows = allShows
                            .Where(s => s.EntityRecord.Id == e.Id && !s.IsReference)
                            .OrderBy(s => s.StartTime)
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