namespace SmartWalk.Server.Records
{
    public class EventMappingRecord
    {
        public virtual int Id { get; set; }
        public virtual EventMetadataRecord EventMetadataRecord { get; set; }
        public virtual StorageRecord StorageRecord { get; set; }
        public virtual ShowRecord ShowRecord { get; set; }
    }
}