using SmartWalk.Shared.DataContracts;

namespace SmartWalk.Labs.Api.DataContracts
{
    public class EventVenueDetail : IEventVenueDetail
    {
        public Reference[] Event { get; set; }
        public Reference[] Venue { get; set; }

        public int? SortOrder { get; set; }
        public string Description { get; set; }

        IReference[] IEventVenueDetail.Event { 
            get { return Event; } 
            set { Event = (Reference[])value; }
        }

        IReference[] IEventVenueDetail.Venue { 
            get { return Venue; } 
            set { Venue = (Reference[])value; }
        }
    }
}