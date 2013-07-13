using MonoTouch.MapKit;
using MonoTouch.CoreLocation;
using SmartWalk.Core.Model;

namespace SmartWalk.iOS.Utils
{
    public class VenueAnnotation : MKAnnotation
    {
        private readonly int _number;
        private readonly EntityInfo _venueInfo;
        private readonly AddressInfo _addressInfo;

        public VenueAnnotation(int number, EntityInfo venueInfo, AddressInfo addressInfo)
        {
            _number = number;
            _venueInfo = venueInfo;
            _addressInfo = addressInfo;

            Coordinate = new CLLocationCoordinate2D(_addressInfo.Latitude, _addressInfo.Longitude);
        }

        public override CLLocationCoordinate2D Coordinate { get; set; }

        public override string Title
        {
            get
            {
                return (_number != 0 ? _number + ". " : string.Empty) + _venueInfo.Name;
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