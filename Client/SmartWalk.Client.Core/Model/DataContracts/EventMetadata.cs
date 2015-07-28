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
        public Pictures Pictures { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public CombineType? CombineType { get; set; }
        public VenueOrderType? VenueOrderType { get; set; }
        public VenueTitleFormatType? VenueTitleFormatType { get; set; }

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
            var em = obj as EventMetadata;
            if (em != null)
            {
                var result = 
                    Id == em.Id &&
                    Host.EnumerableEquals(em.Host) &&
                    Title == em.Title &&
                    Description == em.Description &&
                    Picture == em.Picture &&
                    Equals(Pictures, em.Pictures) &&
                    StartTime == em.StartTime &&
                    EndTime == em.EndTime &&
                    Latitude == em.Latitude &&
                    Longitude == em.Longitude &&
                    CombineType == em.CombineType &&
                    Shows.EnumerableEquals(em.Shows);
                return result;
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
                    .CombineHashCodeOrDefault(Pictures)
                    .CombineHashCode(StartTime)
                    .CombineHashCode(EndTime)
                    .CombineHashCode(Latitude)
                    .CombineHashCode(Longitude)
                    .CombineHashCode(CombineType)
                    .CombineHashCodeOrDefault(Shows);
        }
    }
}