using SmartWalk.Shared.DataContracts;

namespace SmartWalk.Client.Core.Services
{
    public interface IConfiguration
    {
        string Host { get; }
        string Api { get; }

        string GetEventUrl(int eventId);
        string GetEntityUrl(int entityId, EntityType type);
    }
}