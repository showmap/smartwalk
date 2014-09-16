using System;
using SmartWalk.Shared.DataContracts;

namespace SmartWalk.Client.Core.Services
{
    public interface IConfiguration
    {
        string Host { get; }
        string Api { get; }
        ICacheConfiguration CacheConfig { get; }
        TimeSpan PostponeTime { get; }

        string GetEventUrl(int eventId);
        string GetEntityUrl(int entityId, EntityType type);
    }

    public interface ICacheConfiguration
    {
        string CacheFolderPath { get;set; }
        string CacheName { get;set; }
        TimeSpan MaxFileAge { get;set; }
        int MaxFiles { get;set; }
        int MaxInMemoryBytes { get;set; }
        int MaxInMemoryFiles { get;set; }
    }
}