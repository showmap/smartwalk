using System.Collections.Generic;

namespace SmartWalk.Server.Records
{
    public class EntityRecord
    {
        private IList<EntityMappingRecord> _entityMappingRecords;
        private IList<AddressRecord> _addressRecords;
        private IList<ContactRecord> _contactRecords;

        public virtual int Id { get; set; }
        public virtual SmartWalkUserRecord SmartWalkUserRecord { get; set; }
        public virtual int Type { get; set; }
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual string Picture { get; set; }

        public virtual IList<EntityMappingRecord> EntityMappingRecords
        {
            get { return _entityMappingRecords; }
            set { _entityMappingRecords = value; }
        }

        public virtual IList<AddressRecord> AddressRecords
        {
            get { return _addressRecords; }
            set { _addressRecords = value; }
        }

        public virtual IList<ContactRecord> ContactRecords
        {
            get { return _contactRecords; }
            set { _contactRecords = value; }
        }

        public EntityRecord()
        {
            _entityMappingRecords = new List<EntityMappingRecord>();
            _addressRecords = new List<AddressRecord>();
            _contactRecords = new List<ContactRecord>();
        }
    }

    public enum EntityType
    {
        Host = 0,
        Venue = 1
    }
}