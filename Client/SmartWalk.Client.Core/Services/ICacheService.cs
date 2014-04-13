namespace SmartWalk.Client.Core.Services
{
    public interface ICacheService
    {
        string GetString(string key);

        T GetObject<T>(string key);

        void SetString(string key, string value);

        void SetObject<T>(string key, T obj) where T : class;
    }
}