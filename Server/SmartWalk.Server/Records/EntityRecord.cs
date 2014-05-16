using System;
using System.Collections.Generic;
using Orchard.Data.Conventions;

namespace SmartWalk.Server.Records
{
    public class EntityRecord
    {
        private IList<EntityMappingRecord> _entityMappingRecords;
        private IList<AddressRecord> _addressRecords;
        private IList<ContactRecord> _contactRecords;
        private IList<EventMetadataRecord> _eventMetadataRecords;
        private IList<ShowRecord> _showRecords;

        public virtual int Id { get; set; }
        public virtual SmartWalkUserRecord SmartWalkUserRecord { get; set; }
        public virtual int Type { get; set; }
        public virtual string Name { get; set; }
        [StringLengthMax]
        public virtual string Description { get; set; }
        public virtual string Picture { get; set; }
        public virtual bool IsDeleted { get; set; }
        public virtual DateTime DateCreated { get; set; }
        public virtual DateTime DateModified { get; set; }

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

        public virtual IList<EventMetadataRecord> EventMetadataRecords
        {
            get { return _eventMetadataRecords; }
            set { _eventMetadataRecords = value; }
        }

        public virtual IList<ShowRecord> ShowRecords
        {
            get { return _showRecords; }
            set { _showRecords = value; }
        }

        public EntityRecord()
        {
            _entityMappingRecords = new List<EntityMappingRecord>();
            _addressRecords = new List<AddressRecord>();
            _contactRecords = new List<ContactRecord>();
            _eventMetadataRecords = new List<EventMetadataRecord>();
            _showRecords = new List<ShowRecord>();
        }
    }

    public enum EntityType
    {
        Host = 0,
        Venue = 1
    }
}