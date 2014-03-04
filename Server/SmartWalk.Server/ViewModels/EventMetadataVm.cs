using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartWalk.Server.ViewModels
{
    public class EventMetadataVm
    {
        public int Id { get; set; }
        public int HostId { get; set; }
        public int RegionId { get; set; }
        public string HostName { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public int CombineType { get; set; }
        public bool IsMobileReady { get; set; }
        public bool IsWidgetReady { get; set; }
        public string DateCreated { get; set; }
        public string DateModified { get; set; }
    }
}