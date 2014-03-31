using System;
using System.Collections.Generic;

namespace SmartWalk.Server.Records
{
    public class EventMetadataRecord
    {
        private IList<EventMappingRecord> _eventMappingRecords;
        private IList<ShowRecord> _showRecords;

        public virtual int Id { get; set; }        
        public virtual EntityRecord EntityRecord { get; set; } 
        public virtual SmartWalkUserRecord SmartWalkUserRecord { get; set; }
        public virtual string Title { get; set; }
        public virtual string Description { get; set; }
        public virtual DateTime StartTime { get; set; }
        public virtual DateTime? EndTime { get; set; }
        public virtual string Picture { get; set; }
        public virtual double Latitude { get; set; }
        public virtual double Longitude { get; set; }
        public virtual int CombineType { get; set; }
        public virtual bool IsPublic { get; set; }
        public virtual bool IsDeleted { get; set; }
        public virtual DateTime DateCreated { get; set; }
        public virtual DateTime DateModified { get; set; }

        public virtual IList<EventMappingRecord> EventMappingRecords
        {
            get { return _eventMappingRecords; }
            set { _eventMappingRecords = value; }
        }

        public virtual IList<ShowRecord> ShowRecords
        {
            get { return _showRecords; }
            set { _showRecords = value; }
        }

        public EventMetadataRecord()
        {
            _eventMappingRecords = new List<EventMappingRecord>();
            _showRecords = new List<ShowRecord>();
        }
    }

    public enum CombineType {
        None = 0,
        ByVenue = 1,
        ByHost = 2
    }
}