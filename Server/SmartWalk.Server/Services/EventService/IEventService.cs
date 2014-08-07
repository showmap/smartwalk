using System;
using System.Collections.Generic;
using Orchard;
using SmartWalk.Server.Records;
using SmartWalk.Server.Utils;
using SmartWalk.Server.ViewModels;

namespace SmartWalk.Server.Services.EventService
{
    public interface IEventService : IDependency
    {
        IList<EventMetadataVm> GetEvents(
            DisplayType display,
            int pageNumber,
            int pageSize,
            Func<EventMetadataRecord, IComparable> orderBy,
            bool isDesc = false,
            string searchString = null);

        IList<EventMetadataVm> GetEventsByEntity(int entityId);

        /// <summary>
        /// Gets an event by id and filters shows by day.
        /// </summary>
        /// <param name="id">The id of the event.</param>
        /// <param name="day">The day to filter shows by. If Null all shows are returned. 
        /// The day values start from 0 (day 1) and up.</param>
        EventMetadataVm GetEventById(int id, int? day = null);
        AccessType GetEventAccess(int eventId);

        EventMetadataVm SaveEvent(EventMetadataVm eventVm);
        void DeleteEvent(int eventId);
    }
}