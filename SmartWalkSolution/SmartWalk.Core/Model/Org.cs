using System.Collections.Generic;

namespace SmartWalk.Core.Model
{
    public class Org : Entity
    {
        public IEnumerable<OrgEventInfo> EventInfos { get; set; }
    }
}