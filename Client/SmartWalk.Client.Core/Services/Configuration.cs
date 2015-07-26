using System;
using SmartWalk.Shared.DataContracts;

namespace SmartWalk.Client.Core.Services
{
    public class Configuration : IConfiguration
    {
        private const string ApiFormat = "http://{0}/api";
        private const string EventFormat = "http://{0}/event/{1}";
        private const string HostFormat = "http://{0}/organizer/{1}";
        private const string VenueFormat = "http://{0}/venue/{1}";

        public Configuration(string host, string documentsPath, 
            TimeSpan postponeTime, ICacheConfiguration cacheConfig)
        {
            if (host == null) throw new ArgumentNullException("host");
            if (documentsPath == null) throw new ArgumentNullException("documentsPath");
            if (cacheConfig == null) throw new ArgumentNullException("cacheConfig");

            Host = host;
            Api = string.Format(ApiFormat, Host);
            DocumentsPath = documentsPath;
            PostponeTime = postponeTime;
            CacheConfig = cacheConfig;
        }

        public string Host { get; private set; }
        public string Api { get; private set; }
        public string DocumentsPath { get; private set; }
        public ICacheConfiguration CacheConfig { get; private set; }
        public TimeSpan PostponeTime { get; private set; }

        public string GetEventUrl(int eventId)
        {
            var result = string.Format(EventFormat, Host, eventId);
            return result;
        }

        public string GetEntityUrl(int entityId, EntityType type)
        {
            var result = string.Format(
                type == EntityType.Host 
                    ? HostFormat 
                    : VenueFormat, 
                Host, 
                entityId);
            return result;
        }
    }
}