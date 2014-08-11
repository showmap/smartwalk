using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using SmartWalk.Server.Records;
using SmartWalk.Shared;

namespace SmartWalk.Server.Handlers
{
    [UsedImplicitly]
    public class SmartWalkUserHandler : ContentHandler
    {
        public SmartWalkUserHandler(IRepository<SmartWalkUserRecord> repository)
        {
            Filters.Add(StorageFilter.For(repository));
        }
    }
}