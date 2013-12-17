using MonoTouch.CoreLocation;
using MonoTouch.MapKit;
using SmartWalk.Core.Model;
using SmartWalk.iOS.Utils.Map;

namespace SmartWalk.iOS.Utils.Map
{
    public class MapViewAnnotation : MKAnnotation, IMapAnnotation
    {
        private readonly int _number;
        private readonly string _title;
        private readonly string _subTitle;

        public MapViewAnnotation(int number, string title, AddressInfo addressInfo)
        {
            _number = number;
            _title = MapUtil.GetAnnotationTitle(number, title);
            _subTitle = addressInfo.Address;

            Coordinate = new CLLocationCoordinate2D(
                addressInfo.Latitude,
                addressInfo.Longitude);
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