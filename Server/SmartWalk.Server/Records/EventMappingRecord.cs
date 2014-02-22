namespace SmartWalk.Server.Records
{
    public class EventMappingRecord
    {
        public virtual int Id { get; set; }
        public virtual EventMetadataRecord EventMetadataRecord { get; set; }
        public virtual StorageRecord StorageRecord { get; set; }
        public virtual ShowRecord ShowRecord { get; set; } // TODO: Here must be ShowId property with an Id of a show in different storages
    }
}