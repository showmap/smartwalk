using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartWalk.Server.ViewModels
{
    public class EventMetadataFullVm
    {
        public EventMetadataVm EventMetadata { get; set; }
        public IList<EntityVm> Hosts { get; set; }        
        public IList<EntityVm> Venues { get; set; }

        public EventMetadataFullVm() {
            Hosts = new List<EntityVm>();
            Venues = new List<EntityVm>();
        }
    }
}