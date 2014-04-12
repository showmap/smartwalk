using System.Collections.Generic;
using System.Linq;
using MonoTouch.AddressBook;
using MonoTouch.CoreLocation;
using MonoTouch.Foundation;
using MonoTouch.MapKit;
using SmartWalk.Client.Core.Model.DataContracts;

namespace SmartWalk.Client.iOS.Utils.Map
{
    public static class MapUtil
    {
        public static MKCoordinateRegion CoordinateRegionForCoordinates(CLLocationCoordinate2D coordinate) 
        {
            return MKCoordinateRegion.FromDistance(coordinate, 300, 300);
        }

        public static MKCoordinateRegion CoordinateRegionForCoordinates(
            CLLocationCoordinate2D[] coordinates)
        {
            return CoordinateRegionForCoordinates(coordinates, new MKMapSize(0, 0));
        }

        public static MKCoordinateRegion CoordinateRegionForCoordinates(
            CLLocationCoordinate2D[] coordinates,
            MKMapSize margin) 
        {
            var rect = default(MKMapRect); 

            foreach (var coordinate in coordinates)
            {
                var point = MKMapPoint.FromCoordinate(coordinate);
                var pointRect = new MKMapRect(
                    point.X - margin.Width / 2, 
                    point.Y - margin.Height / 2, 
                    margin.Width, 
                    margin.Height);

                rect = rect != default(MKMapRect) ? MKMapRect.Union(rect, pointRect) : pointRect;
            }

            return MKCoordinateRegion.FromMapRect(rect);
        }

        public static CLLocationCoordinate2D[] GetAnnotationsCoordinates(IEnumerable<MKAnnotation> annotations)
        {
            var coordinates = annotations
                .Select(va => va.Coordinate)
                    .Where(c => (long)c.Latitude != 0 && (long)c.Longitude != 0).ToArray();
            return coordinates;
        }

        public static string GetAnnotationTitle(int number, string title)
        {
            return (number != 0 ? number + ". " : string.Empty) + title;
        }

        public static void OpenAddressInMaps(Address address)
        {
            if (address != null)
            {
                var addressDict = new NSMutableDictionary();
                if (address.AddressText != null)
                {
                    addressDict.Add(ABPersonAddressKey.Street, new NSString(address.AddressText));
                }

                var item = new MKMapItem(
                    new MKPlacemark(
                        new CLLocationCoordinate2D(
                            address.Latitude, 
                            address.Longitude),
                        addressDict));

                item.OpenInMaps(new MKLaunchOptions {
                    DirectionsMode = MKDirectionsMode.Walking
                });
            }
        }
    }
}