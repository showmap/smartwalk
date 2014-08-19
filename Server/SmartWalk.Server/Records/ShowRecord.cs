using System;
using Orchard.Data.Conventions;

namespace SmartWalk.Server.Records
{
    public class ShowRecord
    {
        public virtual int Id { get; set; }
        public virtual EntityRecord EntityRecord { get; set; }
        public virtual EventMetadataRecord EventMetadataRecord { get; set; }
        public virtual bool IsReference { get; set; }
        public virtual string Title { get; set; }

        [StringLengthMax]
        public virtual string Description { get; set; }

        public virtual DateTime? StartTime { get; set; }
        public virtual DateTime? EndTime { get; set; }
        public virtual string Picture { get; set; }
        public virtual string DetailsUrl { get; set; }
        public virtual bool IsDeleted { get; set; }
        public virtual DateTime DateCreated { get; set; }
        public virtual DateTime DateModified { get; set; }
    }
}