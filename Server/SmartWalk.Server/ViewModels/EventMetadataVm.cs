using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartWalk.Server.ViewModels
{
    public class EventMetadataVm
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int HostId { get; set; }
        public string HostName { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string Picture { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int CombineType { get; set; }
        public bool IsPublic { get; set; }
        public string DateCreated { get; set; }
        public string DateModified { get; set; }

        public EntityVm Host { get; set; }
        public IList<EntityVm> AllVenues { get; set; }
        public IList<EntityVm> AllHosts { get; set; }        

        public EventMetadataVm() {
            AllVenues = new List<EntityVm>();
            AllHosts = new List<EntityVm>();
        }
    }
}