using SmartWalk.Shared;

namespace SmartWalk.Server.Records
{
    [UsedImplicitly]
    public class ShowMappingRecord
    {
        public virtual int Id { get; set; }
        public virtual int ExternalEventId { get; set; }
        public virtual ShowRecord ShowRecord { get; set; }
        public virtual StorageRecord StorageRecord { get; set; }
    }
}