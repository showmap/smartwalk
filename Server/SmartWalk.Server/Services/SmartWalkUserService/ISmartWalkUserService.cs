using Orchard;
using Orchard.Security;
using SmartWalk.Server.Models;
using SmartWalk.Server.ViewModels;

namespace SmartWalk.Server.Services.SmartWalkUserService
{
    public interface ISmartWalkUserService : IDependency
    {
        IUser CreateUser(SmartWalkUserParams createUserParams);

        SmartWalkUserVm GetCurrentUser();
        void UpdateCurrentUser(SmartWalkUserVm userVm);
        void RequestVerification();
    }
}