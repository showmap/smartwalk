using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard;
using Orchard.Security;
using SmartWalk.Server.Models;

namespace SmartWalk.Server.Services.SmartWalkUserService
{
    public interface ISmartWalkUserService : IDependency {
        IUser CreateUser(SmartWalkUserParams createUserParams);
    }
}