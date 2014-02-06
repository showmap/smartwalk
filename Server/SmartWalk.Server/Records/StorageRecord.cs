using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartWalk.Server.Records
{
    public class StorageRecord
    {
        public virtual int Id { get; set; }
        public virtual string StorageKey { get; set; }
        public virtual string Description { get; set; }
    }
}