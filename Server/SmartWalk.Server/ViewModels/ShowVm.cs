using System;
using System.Globalization;
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

        [JsonIgnore]
        [UsedImplicitly]
        public bool Destroy { get; set; }

        public string DisplayTime(CultureInfo culture)
        {
            return StartTime.ToString("t", culture) +
                   (EndTime.HasValue ? " - " + EndTime.ToString("t", culture) : string.Empty);
        }
    }
}