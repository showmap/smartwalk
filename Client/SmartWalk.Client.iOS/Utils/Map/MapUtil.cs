using System.Collections.Generic;
using System.Linq;
using AddressBook;
using CoreLocation;
using Foundation;
using MapKit;
using SmartWalk.Client.Core.Model.DataContracts;
using SmartWalk.Client.Core.Utils;
using SmartWalk.Client.Core.Resources;
using UIKit;

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

        public static MKMapType ToMKMapType(this MapType mapType)
        {
            switch (mapType)
            {
                case MapType.Standard:
                    return MKMapType.Standard;

                case MapType.Satellite:
                    return MKMapType.Satellite;

                case MapType.Hybrid:
                    return MKMapType.Hybrid;
            }

            return MKMapType.Standard;
        }

        public static string GetMapTypeButtonLabel(this MapType mapType)
        {
            var result = string.Format(Localization.ChangeMapTo, mapType.GetNextMapType());
            return result;
        }

        // TODO: To remove this hack some day
        /// <summary>
        /// HACK: By some reason iOS 64 bit makes Legal label oversized. Reseting Font help to restore it.
        /// </summary>
        public static void FixLegalLabel(this MKMapView mapView)
        {
            var legalLabel = mapView.Subviews.OfType<UILabel>().FirstOrDefault();
            if (legalLabel != null)
            {
                legalLabel.Font = legalLabel.Font;
            }
        }
    }
}