using Orchard.Security;
using SmartWalk.Server.ViewModels;

namespace SmartWalk.Server.Models
{
    public class SmartWalkUserParams
    {
        public CreateUserParams UserParams { get; private set; }
        public SmartWalkUserVm UserData { get; private set; }

        public SmartWalkUserParams(CreateUserParams userParams, SmartWalkUserVm userData) {
            UserParams = userParams;
            UserData = userData;
        }
    }
}