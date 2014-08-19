using System;
using System.Collections.Generic;
using Orchard.ContentManagement.Records;

namespace SmartWalk.Server.Records
{
    public class SmartWalkUserRecord : ContentPartRecord
    {
        private IList<EntityRecord> _entities;
        private IList<EventMetadataRecord> _eventMetadataRecords;

        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual bool IsVerificationRequested { get; set; }
        public virtual DateTime CreatedAt { get; set; }
        public virtual DateTime LastLoginAt { get; set; }

        public virtual IList<EntityRecord> Entities
        {
            get { return _entities; }
            set { _entities = value; }
        }

        public virtual IList<EventMetadataRecord> EventMetadataRecords
        {
            get { return _eventMetadataRecords; }
            set { _eventMetadataRecords = value; }
        }

        public SmartWalkUserRecord()
        {
            _entities = new List<EntityRecord>();
            _eventMetadataRecords = new List<EventMetadataRecord>();
        }
    }
}