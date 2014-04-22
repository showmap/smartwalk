using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard;
using SmartWalk.Server.Records;
using SmartWalk.Server.Services.EntityService;
using SmartWalk.Server.ViewModels;

namespace SmartWalk.Server.Services.EventService
{
    public interface IEventService : IDependency {
        IList<EventMetadataVm> GetEvents(SmartWalkUserRecord user, int pageNumber, int pageSize, Func<EventMetadataRecord, IComparable> orderBy, bool isDesc);
        EventMetadataVm GetUserEventVmById(SmartWalkUserRecord user, int id);
        AccessType GetEventAccess(SmartWalkUserRecord user, int eventId);
        void DeleteEvent(EventMetadataVm item);
        EventMetadataVm SaveOrAddEvent(EventMetadataVm item);
        IList<EventMetadataVm> GetEntityEvents(int entityId);
    }
}