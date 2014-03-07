using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartWalk.Server.ViewModels
{
    public class EventMetadataFullVm
    {
        public EventMetadataVm EventMetadata { get; set; }
        public IList<RegionVm> Regions { get; set; }
        public IList<EntityVm> Hosts { get; set; }
        public IList<ShowVm> Shows { get; set; }
        public IList<EntityVm> Venues { get; set; }

        public EventMetadataFullVm() {
            Regions = new List<RegionVm>();
            Hosts = new List<EntityVm>();
            Shows = new List<ShowVm>();
            Venues = new List<EntityVm>();
        }
    }
}