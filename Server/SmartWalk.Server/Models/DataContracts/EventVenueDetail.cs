using SmartWalk.Shared.DataContracts;

namespace SmartWalk.Server.Models.DataContracts
{
    public class EventVenueDetail : IEventVenueDetail
    {
        public IReference[] Event { get; set; }
        public IReference[] Venue { get; set; }
        public int? SortOrder { get; set; }
        public string Description { get; set; }
    }
}