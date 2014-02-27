using System;
using SmartWalk.Shared.DataContracts;

namespace SmartWalk.Server.Models.DataContracts
{
    public class Show : IShow
    {
        public int Id { get; set; }
        public IReference[] Venue { get; set; }
        public bool? IsReference { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string Picture { get; set; }
        public string DetailsUrl { get; set; }
    }
}