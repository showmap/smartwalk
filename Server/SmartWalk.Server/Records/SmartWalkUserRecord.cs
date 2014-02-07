using System;
using System.Collections.Generic;
using Orchard.ContentManagement.Records;

namespace SmartWalk.Server.Records
{
    public class SmartWalkUserRecord : ContentPartRecord
    {
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual DateTime CreatedAt { get; set; }
        public virtual DateTime LastLoginAt { get; set; }

        public virtual IList<EntityRecord> Entities { get; set; }

        public SmartWalkUserRecord() {
            Entities = new List<EntityRecord>();
        }
    }
}