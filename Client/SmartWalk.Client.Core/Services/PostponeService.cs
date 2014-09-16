using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SmartWalk.Client.Core.Utils;

namespace SmartWalk.Client.Core.Services
{
    /// <summary>
    /// A simple service that tells if an action should be postponed. 
    /// It uses the simple rule of checking the last time of an action, 
    /// if it's less than a constant waiting time then it offers to postpone it.
    /// </summary>
    public class PostponeService : IPostponeService
    {
        private readonly Dictionary<string, DateTime> _memory = new Dictionary<string, DateTime>();
        private readonly TimeSpan PostponeTime;
        private readonly IReachabilityService _reachabilityService;

        public PostponeService(
            IConfiguration configuration,
            IReachabilityService reachabilityService)
        {
            PostponeTime = configuration.PostponeTime;
            _reachabilityService = reachabilityService;
        }

        public bool ShouldPostpone(string key)
        {
            var result = true;

            if (_memory.ContainsKey(key))
            {
                result = DateTime.Now - _memory[key] < PostponeTime;
            }
            else
            {
                _memory[key] = DateTime.Now;
            }

            // invalidating key on postpone waiting's over
            if (!result)
            {
                Invalidate(key);
            }

            return result;
        }

        public void Invalidate(string key)
        {
            InvalidateAsync(key).ContinueWithThrow();
        }

        private async Task InvalidateAsync(string key)
        {
            var isConnected = await _reachabilityService.GetIsReachable();
            if (isConnected)
            {
                _memory[key] = DateTime.Now;
            }
        }
    }
}