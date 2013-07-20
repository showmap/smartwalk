using MonoTouch.MapKit;
using MonoTouch.CoreLocation;
using SmartWalk.Core.Model;

namespace SmartWalk.iOS.Views.OrgEventView
{
    public class VenueAnnotation : MKAnnotation
    {
        private readonly AddressInfo _addressInfo;

        public VenueAnnotation(Venue venue, AddressInfo addressInfo)
        {
            Venue = venue;
            _addressInfo = addressInfo;

            Coordinate = new CLLocationCoordinate2D(_addressInfo.Latitude, _addressInfo.Longitude);
        }

        public Venue Venue { get; set; }

        public override CLLocationCoordinate2D Coordinate { get; set; }

        public override string Title
        {
            get
            {
                return (Venue.Number != 0 ? Venue.Number + ". " : string.Empty) + Venue.Info.Name;
            }
        }

        public override string Subtitle
        {
            get
            {
                return _addressInfo.Address;
            }
        }
    }
}