using System;
using Newtonsoft.Json;
using SmartWalk.Server.Extensions;
using SmartWalk.Shared;

namespace SmartWalk.Server.ViewModels
{
    public class ShowVm
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string Picture { get; set; }
        public string DetailsUrl { get; set; }

        // TODO: To use the common client-server date format
        [JsonIgnore]
        public string DisplayTime
        {
            get
            {
                return StartTime.ToString("t") +
                       (EndTime.HasValue ? " - " + EndTime.ToString("t") : string.Empty);
            }
        }

        [JsonIgnore]
        [UsedImplicitly]
        public bool Destroy { get; set; }
    }
}