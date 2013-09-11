using MonoTouch.CoreLocation;
using MonoTouch.MapKit;
using SmartWalk.Core.Model;

namespace SmartWalk.iOS.Utils
{
    public class MapViewAnnotation : MKAnnotation
    {
        private readonly string _title;
        private readonly string _subTitle;

        public MapViewAnnotation(int number, string title, AddressInfo addressInfo)
        {
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
    }
}