using System;
using System.Collections.Generic;
using Orchard.Data.Conventions;
using SmartWalk.Server.Utils;
using SmartWalk.Shared;

namespace SmartWalk.Server.Records
{
    public class EntityRecord : IAccessRecord, IPicture
    {
        private IList<EntityMappingRecord> _entityMappingRecords;
        private IList<AddressRecord> _addressRecords;
        private IList<ContactRecord> _contactRecords;
        private IList<EventMetadataRecord> _eventMetadataRecords;
        private IList<ShowRecord> _showRecords;

        [UsedImplicitly]
        public virtual int Id { get; set; }
        public virtual SmartWalkUserRecord SmartWalkUserRecord { get; set; }
        public virtual byte Type { get; set; }
        public virtual string Name { get; set; }

        [StringLengthMax]
        public virtual string Description { get; set; }

        public virtual string Picture { get; set; }
        public virtual bool IsDeleted { get; set; }

        [UsedImplicitly] 
        public virtual DateTime DateCreated { get; set; }

        [UsedImplicitly] 
        public virtual DateTime DateModified { get; set; }

        public virtual IList<EntityMappingRecord> EntityMappingRecords
        {
            get { return _entityMappingRecords; }
            [UsedImplicitly]
            set { _entityMappingRecords = value; }
        }

        [CascadeAllDeleteOrphan]
        public virtual IList<AddressRecord> AddressRecords
        {
            get { return _addressRecords; }
            [UsedImplicitly]
            set { _addressRecords = value; }
        }

        [CascadeAllDeleteOrphan]
        public virtual IList<ContactRecord> ContactRecords
        {
            get { return _contactRecords; }
            [UsedImplicitly] 
            set { _contactRecords = value; }
        }

        public virtual IList<EventMetadataRecord> EventMetadataRecords
        {
            get { return _eventMetadataRecords; }
            [UsedImplicitly] 
            set { _eventMetadataRecords = value; }
        }

        public virtual IList<ShowRecord> ShowRecords
        {
            get { return _showRecords; }
            [UsedImplicitly] 
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