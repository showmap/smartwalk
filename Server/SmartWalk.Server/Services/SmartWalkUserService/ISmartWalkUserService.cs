using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard;
using Orchard.Security;
using SmartWalk.Server.Models;
using SmartWalk.Server.ViewModels;

namespace SmartWalk.Server.Services.SmartWalkUserService
{
    public interface ISmartWalkUserService : IDependency {
        IUser CreateUser(SmartWalkUserParams createUserParams);
        SmartWalkUserVm GetUserViewModel(IUser user);
        void UpdateSmartWalkUser(SmartWalkUserVm profile, IUser user);
    }
}