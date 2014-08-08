using System;
using Newtonsoft.Json;
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
    }
}