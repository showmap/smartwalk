using MonoTouch.MapKit;
using MonoTouch.CoreLocation;

namespace SmartWalk.iOS.Utils
{
    public static class MapUtil
    {
        public static MKCoordinateRegion CoordinateRegionForCoordinates(CLLocationCoordinate2D coordinate) 
        {
            return MKCoordinateRegion.FromDistance(coordinate, 300, 300);
        }

        public static MKCoordinateRegion CoordinateRegionForCoordinates(CLLocationCoordinate2D[] coordinates) 
        {
            var rect = default(MKMapRect);

            foreach (var coordinate in coordinates)
            {
                var point = MKMapPoint.FromCoordinate(coordinate);
                var pointRect = new MKMapRect(point.X, point.Y, 0, 0);

                rect = rect != default(MKMapRect) ? MKMapRect.Union(rect, pointRect) : pointRect;
            }

            return MKCoordinateRegion.FromMapRect(rect);
        }
    }
}