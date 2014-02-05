using System;

namespace SmartWalk.Server.Records
{
    public class ShowRecord
    {
        public virtual int Id { get; set; }
        public virtual EntityRecord EntityRecord { get; set; } // TODO: to rename to Venue?
        public virtual string Title { get; set; }
        public virtual string Description { get; set; }
        public virtual DateTime StartTime { get; set; }
        public virtual DateTime EndTime { get; set; }
        public virtual string Picture { get; set; }
        public virtual string DetailsUrl { get; set; }
    }
}