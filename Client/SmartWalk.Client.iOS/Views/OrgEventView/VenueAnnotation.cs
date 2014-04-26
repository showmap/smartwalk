using SmartWalk.Client.Core.Model;
using SmartWalk.Client.Core.Model.DataContracts;
using SmartWalk.Client.iOS.Utils.Map;

namespace SmartWalk.Client.iOS.Views.OrgEventView
{
    public class VenueAnnotation : MapViewAnnotation
    {
        public VenueAnnotation(Venue venue, Address address)
            : base(venue.Info.Name, address)
        {
            Venue = venue;
        }

        public Venue Venue { get; set; }
    }
}