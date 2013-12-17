namespace SmartWalk.Core.Services
{
    public interface ICacheService
    {
        string GetString(string key);

        void SetString(string key, string value);
    }
}