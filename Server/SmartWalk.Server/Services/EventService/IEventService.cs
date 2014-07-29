using System;
using System.Collections.Generic;
using Orchard;
using SmartWalk.Server.Records;
using SmartWalk.Server.Services.EntityService;
using SmartWalk.Server.ViewModels;

namespace SmartWalk.Server.Services.EventService
{
    public interface IEventService : IDependency
    {
        IList<EventMetadataVm> GetEvents(
            SmartWalkUserRecord user,
            int pageNumber,
            int pageSize,
            Func<EventMetadataRecord, IComparable> orderBy,
            bool isDesc = false,
            string searchString = null);

        EventMetadataVm GetEventById(int id, int? day = null);

        AccessType GetEventAccess(SmartWalkUserRecord user, int eventId);

        void DeleteEvent(int eventId);

        EventMetadataVm SaveOrAddEvent(SmartWalkUserRecord user, EventMetadataVm item);

        IList<EventMetadataVm> GetEntityEvents(int entityId);
    }
}