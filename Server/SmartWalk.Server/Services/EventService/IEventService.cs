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
    }
}