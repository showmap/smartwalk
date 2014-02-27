using System.Collections.Generic;
namespace SmartWalk.Server.Records
{
    public class EntityRecord
    {
        public virtual int Id { get; set; }
        public virtual SmartWalkUserRecord SmartWalkUserRecord { get; set; }
        public virtual int Type { get; set; }
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual string Picture { get; set; }

        public virtual IList<EntityMappingRecord> EntityMappingRecords { get; set; }
        public virtual IList<AddressRecord> AddressRecords { get; set; }
        public virtual IList<ContactRecord> ContactRecords { get; set; }

        public EntityRecord() {
            EntityMappingRecords = new List<EntityMappingRecord>();
            AddressRecords = new List<AddressRecord>();
            ContactRecords = new List<ContactRecord>();
        }
    }

    public enum EntityType {
        Host = 0,
        Venue = 1
    }
}