using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartWalk.Server.Records
{
    public class EntityMappingRecord
    {
        public virtual int Id { get; set; }
        public virtual EntityRecord EntityRecord { get; set; }
        public virtual StorageRecord StorageRecord { get; set; }
        public virtual int ExternalEntityId { get; set; }
        public virtual int Type { get; set; }
    }

    public enum EntityMappingType {
        Page = 0,
        Place = 1,
        User = 2,
        Group = 3
    }
}