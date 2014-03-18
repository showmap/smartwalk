using System;
using SmartWalk.Shared.DataContracts;

namespace SmartWalk.Server.Models.DataContracts
{
    public class EventMetadata : IEventMetadata
    {
        public int Id { get; set; }
        public IReference[] Host { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public CombineType? CombineType { get; set; }
        public IReference[] Shows { get; set; }
    }
}