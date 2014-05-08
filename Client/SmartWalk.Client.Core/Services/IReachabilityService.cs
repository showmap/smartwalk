using System;
using System.Threading.Tasks;

namespace SmartWalk.Client.Core.Services
{
    public interface IReachabilityService
    {
        event EventHandler StateChanged;

        Task<bool> GetIsReachable();
    }
}