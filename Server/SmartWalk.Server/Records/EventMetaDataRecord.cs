using System;
using System.Collections.Generic;
using Orchard.Data.Conventions;
using SmartWalk.Server.Utils;
using SmartWalk.Shared;

namespace SmartWalk.Server.Records
{
    public class EventMetadataRecord : IAccessRecord
    {
        private IList<EventMappingRecord> _eventMappingRecords;
        private IList<ShowRecord> _showRecords;
        private IList<EventEntityDetailRecord> _eventEntityDetailRecords;
            
        [UsedImplicitly]
        public virtual int Id { get; set; }
        public virtual EntityRecord EntityRecord { get; set; }
        public virtual SmartWalkUserRecord SmartWalkUserRecord { get; set; }
        public virtual string Title { get; set; }

        [StringLengthMax]
        public virtual string Description { get; set; }

        public virtual DateTime? StartTime { get; set; }
        public virtual DateTime? EndTime { get; set; }
        public virtual string Picture { get; set; }
        public virtual double Latitude { get; set; }
        public virtual double Longitude { get; set; }
        public virtual byte CombineType { get; set; }
        public virtual byte Status { get; set; }
        public virtual bool IsDeleted { get; set; }

        public virtual byte VenueOrderType { get; set; }

        [UsedImplicitly]
        public virtual DateTime DateCreated { get; set; }

        [UsedImplicitly]
        public virtual DateTime DateModified { get; set; }

        public virtual IList<EventMappingRecord> EventMappingRecords
        {
            get { return _eventMappingRecords; }
            set { _eventMappingRecords = value; }
        }

        [CascadeAllDeleteOrphan]
        public virtual IList<ShowRecord> ShowRecords
        {
            get { return _showRecords; }
            [UsedImplicitly]
            set { _showRecords = value; }
        }

        [CascadeAllDeleteOrphan]
        public virtual IList<EventEntityDetailRecord> EventEntityDetailRecords
        {
            get { return _eventEntityDetailRecords; }
            [UsedImplicitly]
            set { _eventEntityDetailRecords = value; }
        }

        public EventMetadataRecord()
        {
            _eventMappingRecords = new List<EventMappingRecord>();
            _showRecords = new List<ShowRecord>();
            _eventEntityDetailRecords = new List<EventEntityDetailRecord>();
        }
    }

    public enum VenueOrderType
    {
        Name = 0,
        Custom = 1
    }

    public enum CombineType
    {
        None = 0,
        [UsedImplicitly]
        ByVenue = 1,
        [UsedImplicitly]
        ByHost = 2
    }

    public enum EventStatus
    {
        Private = 0,
        Public = 1,
        Unlisted = 2
    }
}