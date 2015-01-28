using SystemConfiguration;

namespace SmartWalk.Client.iOS.Utils
{
    // Taken from https://github.com/xamarin/monotouch-samples/blob/master/ReachabilitySample/reachability.cs
    public static class Reachability
    {
        public static bool IsReachableWithoutRequiringConnection(NetworkReachabilityFlags flags)
        {
            // Is it reachable with the current network configuration?
            bool isReachable = (flags & NetworkReachabilityFlags.Reachable) != 0;

            // Do we need a connection to reach it?
            bool noConnectionRequired = (flags & NetworkReachabilityFlags.ConnectionRequired) == 0;

            // Since the network stack will automatically try to get the WAN up, probe that
            if ((flags & NetworkReachabilityFlags.IsWWAN) != 0)
            {
                noConnectionRequired = true;
            }

            return isReachable && noConnectionRequired;
        }

        // Is the host reachable with the current network configuration
        public static bool IsHostReachable(string host)
        {
            if (string.IsNullOrEmpty(host)) return false;

            using (var r = new NetworkReachability(host))
            {
                NetworkReachabilityFlags flags;

                if (r.TryGetFlags(out flags))
                {
                    return IsReachableWithoutRequiringConnection(flags);
                }
            }

            return false;
        }
    }
}