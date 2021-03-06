﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security;
using Orchard;
using Orchard.Data;
using Orchard.FileSystems.Media;
using SmartWalk.Server.Records;
using SmartWalk.Server.Services.Base;
using SmartWalk.Server.Utils;
using SmartWalk.Server.ViewModels;
using SmartWalk.Server.Views;
using SmartWalk.Shared;
using SmartWalk.Shared.Utils;

namespace SmartWalk.Server.Services.EventService
{
    [UsedImplicitly]
    public class EventService : OrchardBaseService, IEventService
    {
        private readonly IRepository<EventMetadataRecord> _eventMetadataRepository;
        private readonly IRepository<EntityRecord> _entityRepository;
        private readonly IStorageProvider _storageProvider;

        public EventService(
            IOrchardServices orchardServices,
            IRepository<EventMetadataRecord> eventMetadataRepository,
            IRepository<EntityRecord> entityRepository,
            IStorageProvider storageProvider)
            : base(orchardServices)
        {
            _eventMetadataRepository = eventMetadataRepository;
            _entityRepository = entityRepository;
            _storageProvider = storageProvider;
        }

        public AccessType GetEventsAccess()
        {
            var result = SecurityUtil.GetAccess(Services.Authorizer);
            return result;
        }

        public AccessType GetEventAccess(int eventId)
        {
            var eventMeta = _eventMetadataRepository.Get(eventId);
            var access = eventMeta.GetAccess(Services.Authorizer, CurrentUserRecord);
            var result =
                eventMeta.Status == (byte)EventStatus.Private
                && access != AccessType.AllowEdit
                    ? AccessType.Deny
                    : access;
            return result;
        }

        public IList<EventMetadataVm> GetEvents(
            DisplayType display, int pageNumber, int pageSize,
            SortType sort, bool isDesc = false,
            string searchString = null)
        {
            var access = GetEventsAccess();
            if (access == AccessType.Deny)
                throw new SecurityException("Can't get events.");

            if (display == DisplayType.My && CurrentUserRecord == null)
                throw new SecurityException("Can't show my events without user.");

            var query =
                display == DisplayType.All
                    ? _eventMetadataRepository
                        .Table
                        .Where(e => e.Status == (byte)EventStatus.Public)
                    : CurrentUserRecord.EventMetadataRecords.AsQueryable();

            var result = GetEvents(query, pageNumber, pageSize, 
                sort, isDesc, searchString);
            return result;
        }

        public IList<EventMetadataVm> GetEventsByEntity(int entityId)
        {
            var access = GetEventsAccess();
            if (access == AccessType.Deny)
                throw new SecurityException("Can't get events.");

            var entity = _entityRepository.Get(entityId);
            if (entity == null || entity.IsDeleted) return null;

            IQueryable<EventMetadataRecord> query;

            if ((EntityType)entity.Type == EntityType.Venue)
            {
                query = _eventMetadataRepository
                    .Table
                    .Where(md => md.ShowRecords.Any(s => s.EntityRecord.Id == entityId));
            }
            else
            {
                query = entity.EventMetadataRecords.AsQueryable();
            }

            var result = GetEvents(query, 0, ViewSettings.RelatedItems, SortType.Date, true);
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

            var access = eventMeta.GetAccess(Services.Authorizer, CurrentUserRecord);
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
                    && host.SmartWalkUserRecord.Id != CurrentUserRecord.Id) 
                throw new ArgumentOutOfRangeException("eventVm.Host", "Can't add host created by other user to the event.");

            var eventMeta = _eventMetadataRepository.Get(eventVm.Id) ?? new EventMetadataRecord
                {
                    SmartWalkUserRecord = CurrentUserRecord,
                    DateCreated = DateTime.UtcNow
                };

            var access = eventMeta.GetAccess(Services.Authorizer, CurrentUserRecord);
            if (access != AccessType.AllowEdit)
                throw new SecurityException("Can't edit event.");

            if (eventMeta.IsDeleted)
                throw new InvalidOperationException("Can't edit deleted event.");

            if (eventMeta.Status == (byte)EventStatus.Public
                && !Services.Authorizer.Authorize(Permissions.CreatePublicContent))
                throw new SecurityException("Current user can not make public events.");

            ViewModelFactory.UpdateByViewModel(eventMeta, eventVm, host);

            var venues = CompressVenues(eventVm.Venues);
            var showsDict = new Dictionary<ShowRecord, ShowVm>();
            foreach (var venueVm in venues)
            {
                var venue = _entityRepository.Get(venueVm.Id);
                if (venue == null) continue;

                SaveVenueDetail(eventMeta, venue, venueVm);

                foreach (var showVm in venueVm.Shows)
                {
                    // ReSharper disable once AccessToForEachVariableInClosure
                    var show = showVm.Id > 0 
                        ? eventMeta.ShowRecords.FirstOrDefault(s => s.Id == showVm.Id)
                        : null;
                    if (show == null && !venueVm.Destroy)
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
                    if (show != null && venueVm.Destroy)
                    {
                        showVm.Destroy = true;
                    }

                    if (show != null)
                    {
                        show.EntityRecord = venue; // in case if shows moved to another venue
                        ViewModelFactory.UpdateByViewModel(show, showVm);

                        if (!show.IsDeleted)
                        {
                            showsDict.Add(show, showVm);
                        }
                    }
                }

                UpdateReferenceShow(eventMeta, venue, 
                    !venueVm.Destroy && venueVm.Shows.Count(s => !s.Destroy) == 0);
            }

            DestroyMissingVenues(eventMeta, venues);
            RecalcEventCoordinates(eventMeta);

            if (eventMeta.Id == 0)
            {
                _eventMetadataRepository.Create(eventMeta);
            }

            // handling uploaded pictures after event got an Id
            ViewModelFactory.UpdateByViewModelPicture(eventMeta, eventVm, showsDict, _storageProvider);

            _eventMetadataRepository.Flush();

            var result = CreateViewModelContract(eventMeta, null);
            return result;
        }

        public void DeleteEvent(int eventId)
        {
            var eventMeta = _eventMetadataRepository.Get(eventId);
            if (eventMeta == null || eventMeta.IsDeleted) return;

            var access = eventMeta.GetAccess(Services.Authorizer, CurrentUserRecord);
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

        private static void SaveVenueDetail(EventMetadataRecord eventMeta, EntityRecord venue, EntityVm venueVm)
        {
            // ReSharper disable once AccessToForEachVariableInClosure
            var entityDetail = eventMeta
                .EventEntityDetailRecords
                .FirstOrDefault(eedr => eedr.EntityRecord.Id == venueVm.Id);

            if (venueVm.EventDetail != null)
            {
                entityDetail = entityDetail ?? new EventEntityDetailRecord
                    {
                        EntityRecord = venue,
                        EventMetadataRecord = eventMeta
                    };

                EntityService
                    .ViewModelFactory
                    .UpdateByViewModel(entityDetail, venueVm.EventDetail);

                if (entityDetail.Id == 0)
                {
                    eventMeta.EventEntityDetailRecords.Add(entityDetail);
                }

                entityDetail.IsDeleted = venueVm.Destroy;
            }
            else if (entityDetail != null)
            {
                entityDetail.IsDeleted = true;
            }
        }

        private IList<EventMetadataVm> GetEvents(
            IQueryable<EventMetadataRecord> query, int pageNumber, int pageSize,
            SortType sort, bool isDesc = false,
            string searchString = null)
        {
            query = query.Where(e => !e.IsDeleted);

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                query = query
                    .Where(e => (e.Title != null
                                 && e.Title.ToUpperInvariant().Contains(searchString.ToUpperInvariant()))
                                ||
                                (e.Title == null
                                 && e.EntityRecord.Name.ToUpperInvariant().Contains(searchString.ToUpperInvariant())));
            }

            query = isDesc
                ? (sort == SortType.Date 
                    ? query.OrderByDescending(e => e.StartTime)
                    : query.OrderByDescending(e => e.Title ?? e.EntityRecord.Name))
                : (sort == SortType.Date
                    ? query.OrderBy(e => e.StartTime)
                    : query.OrderBy(e => e.Title ?? e.EntityRecord.Name));

            var result =
                query.Skip(pageSize * pageNumber)
                     .Take(pageSize)
                     .Select(e => CreateViewModelContract(e, null, LoadMode.Compact))
                     .ToArray();
            return result;
        }

        private EventMetadataVm CreateViewModelContract(
            EventMetadataRecord eventMeta,
            int? day,
            LoadMode mode = LoadMode.Full)
        {
            if (eventMeta == null) throw new ArgumentNullException("eventMeta");

            var result = ViewModelFactory.CreateViewModel(eventMeta, mode, _storageProvider);
            result.Host =
                EntityService
                    .ViewModelFactory.CreateViewModel(eventMeta.EntityRecord, mode, _storageProvider);

            if (mode == LoadMode.Full)
            {
                result.Venues = GetEventVenues(eventMeta, day);
            }

            return result;
        }

        private IList<EntityVm> GetEventVenues(EventMetadataRecord eventMeta, int? day)
        {
            var range = new Tuple<DateTime?, DateTime?>(eventMeta.StartTime, eventMeta.EndTime);
            var currentDay =
                day == null
                    ? null
                    : (eventMeta.IsMultiDay() && eventMeta.StartTime.HasValue
                        ? (DateTime?)eventMeta.StartTime.Value.AddDays(day.Value)
                        : null);

            var allShows =
                eventMeta.ShowRecords
                    .Where(s =>
                        !s.IsDeleted &&
                        (s.IsReference || s.StartTime.IsTimeThisDay(currentDay, range)))
                    .ToArray();

            var venues = allShows
                .Select(s => s.EntityRecord)
                .Distinct()
                .Select(e =>
                    {
                        var venueDetail = eventMeta
                            .EventEntityDetailRecords
                            .FirstOrDefault(eedr => !eedr.IsDeleted && eedr.EntityRecord.Id == e.Id);
                        var venueVm = EntityService
                            .ViewModelFactory
                            .CreateViewModel(e, venueDetail, _storageProvider);

                        venueVm.Shows = allShows
                            .Where(s => s.EntityRecord.Id == e.Id && !s.IsReference)
                            .OrderBy(s => s.StartTime)
                            .Select(s => ViewModelFactory.CreateViewModel(s, _storageProvider))
                            .ToArray();

                        return venueVm;
                    });

            EntityVm[] result;

            if (eventMeta.VenueOrderType == (byte)VenueOrderType.Custom)
            {
                result = venues
                    .OrderBy(v =>
                        v.EventDetail != null &&
                        v.EventDetail.SortOrder != null
                            ? v.EventDetail.SortOrder.Value
                            : 0)
                    .ToArray();
            }
            else if (eventMeta.VenueOrderType == (byte)VenueOrderType.Name)
            {
                result = venues
                    .OrderBy(v => v.Name, NameComparer)
                    .ToArray();
            }
            else
            {
                result = venues.ToArray();
            }

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

        private void DestroyMissingVenues(EventMetadataRecord eventMeta, EntityVm[] venues)
        {
            var eventVenues = GetEventVenues(eventMeta, null);
            var missingVenues = eventVenues
                .Where(ev => venues.All(v => v.Id != ev.Id))
                .ToArray();

            foreach (var venueVm in missingVenues)
            {
                var venue = _entityRepository.Get(venueVm.Id);
                if (venue == null) continue;

                venueVm.Destroy = true;
                SaveVenueDetail(eventMeta, venue, venueVm);
            }

            var missingVenueIds = missingVenues.Select(v => v.Id).ToArray();
            var shows = eventMeta.ShowRecords.Where(s => missingVenueIds.Contains(s.EntityRecord.Id));

            foreach (var show in shows)
            {
                show.IsDeleted = true;
                show.DateModified = DateTime.UtcNow;
            }
        }

        private static EntityVm[] CompressVenues(IList<EntityVm> venues)
        {
            var alive = venues.Where(v => !v.Destroy).ToArray();
            var destroyed = venues.Where(v => v.Destroy).ToList();

            foreach (var venue in destroyed.ToArray())
            {
                // ReSharper disable once AccessToForEachVariableInClosure
                if (alive.Any(v => v.Id == venue.Id))
                {
                    destroyed.Remove(venue);
                }
            }

            var result = alive.Union(destroyed).ToArray();
            return result;
        }
    }
}