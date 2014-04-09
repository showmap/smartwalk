namespace SmartWalk.Client.Core.Services
{
    public interface IReachabilityService
    {
        bool IsHostReachable(string host);
    }
}