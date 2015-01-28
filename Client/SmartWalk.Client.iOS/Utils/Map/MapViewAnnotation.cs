using System;
using CoreLocation;
using MapKit;
using SmartWalk.Client.Core.Model.DataContracts;
using SmartWalk.Client.iOS.Utils.Map;

namespace SmartWalk.Client.iOS.Utils.Map
{
    public class MapViewAnnotation : MKAnnotation, IMapAnnotation
    {
        private readonly string _pin;
        private readonly string _title;
        private readonly string _subTitle;

        private CLLocationCoordinate2D _coordinate;

        public MapViewAnnotation(string pin, string title, Address address)
        {
            if (address == null) throw new ArgumentNullException("address");

            _pin = pin;
            _title = title;
            _subTitle = address.AddressText;

            _coordinate = new CLLocationCoordinate2D(
                address.Latitude,
                address.Longitude);
        }

        public override CLLocationCoordinate2D Coordinate
        { 
            get { return _coordinate; }
        }

        public string Pin 
        {
            get { return _pin; }
        }

        public override string Title
        {
            get { return _title; }
        }

        public override string Subtitle
        {
            get { return _subTitle; }
        }

        public virtual object DataContext
        {
            get { return null; }
        }

        public override void SetCoordinate(CLLocationCoordinate2D value)
        {
            _coordinate = value;
        }
    }
}