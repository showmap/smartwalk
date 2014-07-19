using System;
using System.Collections.Generic;
using System.Globalization;
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

        public string DisplayDate(CultureInfo culture)
        {
            return StartDate.ToString("D", culture) +
                    (EndDate.HasValue ? " - " + EndDate.ToString("D", culture) : string.Empty);
        }
    }
}