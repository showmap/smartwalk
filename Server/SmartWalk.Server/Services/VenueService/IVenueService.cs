using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard;
using SmartWalk.Server.Records;
using SmartWalk.Server.ViewModels;

namespace SmartWalk.Server.Services.VenueService
{
    public interface IVenueService : IDependency
    {
        IList<EntityVm> GetUserVenues(SmartWalkUserRecord user);
    }
}