using System;
using SmartWalk.Shared.DataContracts;

namespace SmartWalk.Labs.Api.DataContracts
{
    public class EventMetadata : IEventMetadata
    {
        public int Id { get; set; }
        public Reference[] Host { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public CombineType? CombineType { get; set; }
        public Reference[] Shows { get; set; }

        IReference[] IEventMetadata.Host
        {
            get { return Host; }
            set { Host = (Reference[])value; }
        }

        IReference[] IEventMetadata.Shows
        {
            get { return Shows; }
            set { Shows = (Reference[])value; }
        }
    }
}
