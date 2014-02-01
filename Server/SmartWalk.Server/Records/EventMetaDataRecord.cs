using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartWalk.Server.Records
{
    public class EventMetadataRecord
    {
        public virtual int Id { get; set; }
        public virtual RegionRecord RegionRecord { get; set; }
        public virtual EntityRecord EntityRecord { get; set; }
        public virtual SmartWalkUserRecord SmartWalkUserRecord { get; set; }
        public virtual string Title { get; set; }
        public virtual string Description { get; set; }
        public virtual DateTime StartTime { get; set; }
        public virtual DateTime EndTime { get; set; }
        public virtual int CombineType { get; set; }
        public virtual bool IsMobileReady { get; set; }
        public virtual bool IsWidgetReady { get; set; }
        public virtual DateTime DateCreated { get; set; }
        public virtual DateTime DateModified { get; set; }
    }

    public enum CombineType {
        None = 0,
        ByVenue = 1,
        ByHost = 2
    }
}