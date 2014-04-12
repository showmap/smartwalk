using System;
using SmartWalk.Shared.DataContracts;
using SmartWalk.Shared.Utils;

namespace SmartWalk.Client.Core.Model.DataContracts
{
    public class EventMetadata : IEventMetadata
    {
        public int Id { get; set; }
        public Reference[] Host { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Picture { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public CombineType? CombineType { get; set; }
        public Reference[] Shows { get; set; }

        IReference[] IEventMetadata.Host { 
            get { return Host; } 
            set { Host = (Reference[])value; }
        }

        IReference[] IEventMetadata.Shows { 
            get { return Shows; } 
            set { Shows = (Reference[])value; }
        }

        public override bool Equals(object obj)
        {
            var eventMetadata = obj as EventMetadata;
            if (eventMetadata != null)
            {
                return Id == eventMetadata.Id;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Initial
                    .CombineHashCode(Id)
                    .CombineHashCodeOrDefault(Host)
                    .CombineHashCodeOrDefault(Title)
                    .CombineHashCodeOrDefault(Description)
                    .CombineHashCodeOrDefault(Picture)
                    .CombineHashCode(StartTime)
                    .CombineHashCode(EndTime)
                    .CombineHashCode(Latitude)
                    .CombineHashCode(Longitude)
                    .CombineHashCode(CombineType)
                    .CombineHashCodeOrDefault(Shows);
        }
    }
}