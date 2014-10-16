using SmartWalk.Client.Core.Model;
using SmartWalk.Client.Core.Model.DataContracts;
using SmartWalk.Client.Core.Utils;
using SmartWalk.Client.iOS.Utils.Map;

namespace SmartWalk.Client.iOS.Views.OrgEventView
{
    public class VenueAnnotation : MapViewAnnotation
    {
        public VenueAnnotation(Venue venue, Address address)
            : base(venue.PinText(), venue.Info.Name, address)
        {
            Venue = venue;
        }

        public Venue Venue { get; set; }

        public override object DataContext
        {
            get { return Venue; }
        }
    }
}