namespace SmartWalk.Server.Records
{
    public class EventEntityDetailRecord
    {
        public virtual int Id { get; set; }
        public virtual EntityRecord EntityRecord { get; set; }
        public virtual EventMetadataRecord EventMetadataRecord { get; set; }
        public virtual int? SortOrder { get; set; }
        public virtual string Description { get; set; }
    }
}