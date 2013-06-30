using System.Collections.Generic;

namespace SmartWalk.Core.Model
{
    public class OrgEvent
    {
        public OrgEventInfo Info { get; set; }

        public IEnumerable<Venue> Venues { get; set; }
    }
}