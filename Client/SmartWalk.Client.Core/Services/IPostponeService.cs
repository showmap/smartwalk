namespace SmartWalk.Client.Core.Services
{
    public interface IPostponeService
    {
        bool ShouldPostpone(string key);
        void Invalidate(string key);
    }
}