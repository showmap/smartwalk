using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SmartWalk.Server.Extensions;

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
        /*public double Latitude { get; set; }
        public double Longitude { get; set; }*/

        public EntityVm Host { get; set; }
        public IList<EntityVm> Venues { get; set; }

        [JsonIgnore]
        public string DisplayName
        {
            get { return Title ?? (Host != null ? Host.Name : null); }
        }

        [JsonIgnore]
        public string DisplayPicture
        {
            get { return Picture ?? (Host != null ? Host.Picture : null); }
        }

        // TODO: To use the common client-server date format
        [JsonIgnore]
        public string DisplayDate
        {
            get
            {
                return StartDate.ToString("D") +
                       (EndDate.HasValue ? " - " + EndDate.ToString("D") : string.Empty);
            }
        }
    }
}