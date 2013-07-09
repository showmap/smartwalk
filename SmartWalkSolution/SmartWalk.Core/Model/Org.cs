using System.Collections.Generic;

namespace SmartWalk.Core.Model
{
    public class Org
    {
        public EntityInfo Info { get; set; }

        public string Description { get; set; }

        public IEnumerable<OrgEventInfo> EventInfos { get; set; }
    }
}