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

        public event EventHandler StateChanged;

        private bool IsReachable
        {
            get { return _isReachable; }
            set 
            {
                if (_isReachable != value)
                {
                    _isReachable = value;

                    if (StateChanged != null)
                    {
                        StateChanged(this, EventArgs.Empty);
                    }
                }
            }
        }

        public async Task<bool> GetIsReachable()
        {
            if (_reachability == null)
            {
                try
                {
                    await Initialize();
                }
                catch
                {
                    _reachability = null;
                    IsReachable = false;
                }
            }

            return IsReachable;
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

        private async Task Initialize()
        {
            _reachability = new NetworkReachability(_configuration.Host);

            var reachability = _reachability;
            var resultFlags = 
                await Task.Run(
                () =>
                {
                    NetworkReachabilityFlags flags;

                    if (reachability.TryGetFlags(out flags))
                    {
                        return flags;
                    }

                    return (NetworkReachabilityFlags)0;
                });

            OnNotification(resultFlags);

            _reachability.SetNotification(OnNotification);
            _reachability.Schedule();
        }

        private void OnNotification(NetworkReachabilityFlags flags)
        {
            IsReachable = Reachability.IsReachableWithoutRequiringConnection(flags);
        }
    }
}