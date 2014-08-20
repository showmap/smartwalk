using System.Collections.Generic;
using Orchard;
using SmartWalk.Server.Utils;
using SmartWalk.Server.ViewModels;

namespace SmartWalk.Server.Services.EventService
{
    public interface IEventService : IDependency
    {
        AccessType GetEventsAccess();
        AccessType GetEventAccess(int eventId);

        IList<EventMetadataVm> GetEvents(
            DisplayType display, int pageNumber, int pageSize,
            SortType sort, bool isDesc = false,
            string searchString = null);
        IList<EventMetadataVm> GetEventsByEntity(int entityId);
        EventMetadataVm GetEventById(int id, int? day = null);
        EventMetadataVm SaveEvent(EventMetadataVm eventVm);
        void DeleteEvent(int eventId);
    }
}