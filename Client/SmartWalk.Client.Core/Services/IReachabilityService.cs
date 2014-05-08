using System.Threading.Tasks;

namespace SmartWalk.Client.Core.Services
{
    public interface IReachabilityService
    {
        Task<bool> GetIsReachable();
    }
}