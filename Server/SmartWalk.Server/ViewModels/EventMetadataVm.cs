using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SmartWalk.Server.ViewModels
{
    public class EventMetadataVm
    {
        public EventMetadataVm()
        {
            Venues = new List<EntityVm>();
        }

        public int Id { get; set; }
        public int CombineType { get; set; }
        public bool IsPublic { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Picture { get; set; }

        // We don't need these on client so far
        [JsonIgnore]
        public double Latitude { get; set; }
        [JsonIgnore]
        public double Longitude { get; set; }

        public EntityVm Host { get; set; }
        public IList<EntityVm> Venues { get; set; }
    }
}