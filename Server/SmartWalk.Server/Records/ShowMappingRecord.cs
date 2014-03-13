using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartWalk.Server.Records
{
    public class ShowMappingRecord
    {
        public virtual int Id { get; set; }
        public virtual int ExternalEventId { get; set; }
        public virtual ShowRecord ShowRecord  { get; set; }
        public virtual StorageRecord StorageRecord { get; set; }
    }
}