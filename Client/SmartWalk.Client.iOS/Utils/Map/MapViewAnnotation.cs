using System;
using MonoTouch.CoreLocation;
using MonoTouch.MapKit;
using SmartWalk.Client.Core.Model.DataContracts;
using SmartWalk.Shared.Utils;
using SmartWalk.Client.iOS.Utils.Map;

namespace SmartWalk.Client.iOS.Utils.Map
{
    public class MapViewAnnotation : MKAnnotation, IMapAnnotation
    {
        private readonly string _title;
        private readonly string _subTitle;

        public MapViewAnnotation(string title, Address address)
        {
            if (address == null) throw new ArgumentNullException("address");

            Abbr = title.GetAbbreviation(2);
            _title = title;
            _subTitle = address.AddressText;

            Coordinate = new CLLocationCoordinate2D(
                address.Latitude,
                address.Longitude);
        }

        public override CLLocationCoordinate2D Coordinate { get; set; }

        public string Abbr { get;set; }

        public override string Title
        {
            get { return _title; }
        }

        public override string Subtitle
        {
            get { return _subTitle; }
        }
    }
}