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
        AccessType GetEventsAccess();
        AccessType GetEventAccess(int eventId);

        IList<EventMetadataVm> GetEvents(
            DisplayType display,
            int pageNumber,
            int pageSize,
            Func<EventMetadataRecord, IComparable> orderBy,
            bool isDesc = false,
            string searchString = null);
        IList<EventMetadataVm> GetEventsByEntity(int entityId);
        EventMetadataVm GetEventById(int id, int? day = null);
        EventMetadataVm SaveEvent(EventMetadataVm eventVm);
        void DeleteEvent(int eventId);
    }
}