using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard;
using SmartWalk.Server.Records;
using SmartWalk.Server.ViewModels;

namespace SmartWalk.Server.Services.EventService
{
    public interface IEventService : IDependency {
        IList<EventMetadataVm> GetUserEvents(SmartWalkUserRecord user);
        EventMetadataVm GetUserEventVmById(SmartWalkUserRecord user, int id);
        void DeleteEvent(EventMetadataVm item);
        EventMetadataVm SaveOrAddEvent(EventMetadataVm item);
        IList<EventMetadataVm> GetEntityEvents(int entityId);
    }
}