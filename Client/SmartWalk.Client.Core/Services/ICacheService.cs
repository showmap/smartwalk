namespace SmartWalk.Client.Core.Services
{
    public interface ICacheService
    {
        string GetString(string key, bool deleteIfOld = true);

        T GetObject<T>(string key, bool deleteIfOld = true);

        void SetString(string key, string value);

        void SetObject<T>(string key, T obj) where T : class;

        void InvalidateString(string key);
    }
}