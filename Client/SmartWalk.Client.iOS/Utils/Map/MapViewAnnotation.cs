using MonoTouch.CoreLocation;
using MonoTouch.MapKit;
using SmartWalk.Client.Core.Model.DataContracts;
using SmartWalk.Client.iOS.Utils.Map;

namespace SmartWalk.Client.iOS.Utils.Map
{
    public class MapViewAnnotation : MKAnnotation, IMapAnnotation
    {
        private readonly int _number;
        private readonly string _title;
        private readonly string _subTitle;

        public MapViewAnnotation(int number, string title, Address address)
        {
            _number = number;
            _title = MapUtil.GetAnnotationTitle(number, title);
            _subTitle = address.AddressText;

            Coordinate = new CLLocationCoordinate2D(
                address.Latitude,
                address.Longitude);
        }

        public override CLLocationCoordinate2D Coordinate { get; set; }

        public override string Title
        {
            get
            {
                return _title;
            }
        }

        public override string Subtitle
        {
            get
            {
                return _subTitle;
            }
        }

        public int Number
        {
            get
            {
                return _number;
            }
        }

        public string Logo
        {
            get
            {
                return null;
            }
        }
    }
}