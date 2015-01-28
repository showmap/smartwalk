using System;
using System.Threading.Tasks;
using SystemConfiguration;
using SmartWalk.Client.Core.Services;
using SmartWalk.Client.Core.Utils;
using SmartWalk.Client.iOS.Utils;

namespace SmartWalk.Client.iOS.Services
{
    public class ReachabilityService : IReachabilityService, IDisposable
    {
        private readonly IConfiguration _configuration;
        private readonly NetworkReachability _reachability;
        private readonly Task _initializeTask;

        private bool _isReachable;

        public ReachabilityService(IConfiguration configuration)
        {
            _configuration = configuration;
            _reachability = new NetworkReachability(_configuration.Host);

            _initializeTask = Task.Run(() =>
                {
                    NetworkReachabilityFlags flags;

                    var resultFlags = _reachability.TryGetFlags(out flags) 
                            ? flags 
                                : (NetworkReachabilityFlags)0;

                    return resultFlags;
                })
                .ContinueWithUIThread(previousTask =>
                    {
                        OnNotification(previousTask.Result);

                        _reachability.SetNotification(OnNotification);
                        _reachability.Schedule();
                    });
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
            await _initializeTask;

            return IsReachable;
        }

        public void Dispose()
        {
            if (_reachability != null)
            {
                _reachability.Unschedule();
                _reachability.Dispose();
            }
        }

        private void OnNotification(NetworkReachabilityFlags flags)
        {
            IsReachable = Reachability.IsReachableWithoutRequiringConnection(flags);
        }
    }
}