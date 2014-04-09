using SmartWalk.Client.Core.Services;
using SmartWalk.Client.iOS.Utils;

namespace SmartWalk.Client.iOS.Services
{
    public class ReachabilityService : IReachabilityService
    {
        public bool IsHostReachable(string host)
        {
            return Reachability.IsHostReachable(host);
        }
    }
}