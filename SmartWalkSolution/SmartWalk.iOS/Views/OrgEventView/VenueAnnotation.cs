using SmartWalk.Core.Model;
using SmartWalk.iOS.Views.Common.EntityCell;

namespace SmartWalk.iOS.Views.OrgEventView
{
    public class VenueAnnotation : EntityAnnotation
    {
        public VenueAnnotation(Venue venue, AddressInfo addressInfo)
            : base(venue, addressInfo)
        {
            Venue = venue;
        }

        public Venue Venue { get; set; }

        public override string Title
        {
            get
            {
                return (Venue.Number != 0 ? Venue.Number + ". " : string.Empty) + Venue.Info.Name;
            }
        }
    }
}