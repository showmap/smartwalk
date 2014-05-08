using System;
using System.Threading.Tasks;
using MonoTouch.SystemConfiguration;
using SmartWalk.Client.Core.Services;
using SmartWalk.Client.iOS.Utils;

namespace SmartWalk.Client.iOS.Services
{
    public class ReachabilityService : IReachabilityService, IDisposable
    {
        private readonly IConfiguration _configuration;

        private bool _isReachable;
        private NetworkReachability _reachability;

        public ReachabilityService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> GetIsReachable()
        {
            if (_reachability == null)
            {
                Initialize();

                await Task.Run(
                    () =>
                    {
                        NetworkReachabilityFlags flags;

                        if (_reachability.TryGetFlags(out flags))
                        {
                            OnNotification(flags);
                        }
                    });
            }

            return _isReachable;
        }

        public void Dispose()
        {
            if (_reachability != null)
            {
                _reachability.Unschedule();
                _reachability.Dispose();
                _reachability = null;
            }
        }

        private void Initialize()
        {
            _reachability = new NetworkReachability(_configuration.Host);
            _reachability.SetNotification(OnNotification);
            _reachability.Schedule();
        }

        private void OnNotification(NetworkReachabilityFlags flags)
        {
            _isReachable = Reachability.IsReachableWithoutRequiringConnection(flags);
        }
    }
}