using System;
using System.Linq;
using System.Threading.Tasks;
using MonoTouch.CoreLocation;
using Refractored.MvxPlugins.Settings;
using SmartWalk.Client.Core.Model;
using SmartWalk.Client.Core.Services;
using SmartWalk.Client.Core.Utils;

namespace SmartWalk.Client.iOS.Services
{
    public class LocationService : ILocationService
    {
        private const string LastLocationLat = "LastLocationLat";
        private const string LastLocationLong = "LastLocationLong";
        private const string UnknownLocation = "Unknown Location";

        private readonly ISettings _settings;
        private readonly IExceptionPolicy _exceptionPolicy;
        private readonly CLLocationManager _locationManager;
        private readonly CLGeocoder _geocoder;

        public LocationService(ISettings settings, IExceptionPolicy exceptionPolicy)
        {
            _settings = settings;
            _exceptionPolicy = exceptionPolicy;

            if (CLLocationManager.LocationServicesEnabled)
            {
                _geocoder = new CLGeocoder();
                _locationManager = new CLLocationManager();
                _locationManager.DesiredAccuracy = CLLocation.AccuracyThreeKilometers;

                _locationManager.LocationsUpdated += (s, e) => 
                {
                    SaveLocationSettings();
                    UpdateLocationString().ContinueWithThrow();

                    if (LocationChanged != null)
                    {
                        LocationChanged(this, EventArgs.Empty);
                    }
                };
            }

            SaveLocationSettings();
            UpdateLocationString().ContinueWithThrow();
        }

        public event EventHandler LocationChanged;
        public event EventHandler LocationStringChanged;

        public Location CurrentLocation 
        { 
            get
            { 
                CLLocationCoordinate2D? coordinate;

                if (_locationManager != null &&
                    _locationManager.Location != null)
                {
                    coordinate = _locationManager.Location.Coordinate;
                }
                else
                {
                    coordinate = LoadLocationSettings();
                }

                if (coordinate.HasValue)
                {
                    var result = new Location(
                        Math.Round(coordinate.Value.Latitude, 2),
                        Math.Round(coordinate.Value.Longitude, 2));
                    return result;
                }

                return Location.Empty;
            }
        }

        public string CurrentLocationString { get; private set; }

        private async Task UpdateLocationString()
        {
            if (CurrentLocation != Location.Empty)
            {
                var placemarks = default(CLPlacemark[]);

                try
                {
                    var location = new CLLocation(
                        CurrentLocation.Latitude, 
                        CurrentLocation.Longitude);
                    placemarks = await _geocoder
                        .ReverseGeocodeLocationAsync(location);
                }
                catch (Exception ex)
                {
                    _exceptionPolicy.Trace(ex, false);
                }

                if (placemarks != null)
                {
                    var placemark = placemarks.FirstOrDefault();
                    if (placemark != null)
                    {
                        CurrentLocationString = string.Format(
                            "{0}, {1}", 
                            placemark.Locality, 
                            placemark.Country);
                    }
                }
            }
            else
            {
                CurrentLocationString = UnknownLocation;
            }

            if (LocationStringChanged != null)
            {
                LocationStringChanged(this, EventArgs.Empty);
            }
        }

        private CLLocationCoordinate2D? LoadLocationSettings()
        {
            var latitude = _settings.GetValueOrDefault<double?>(LastLocationLat);
            var longitude = _settings.GetValueOrDefault<double?>(LastLocationLong);

            if (latitude.HasValue && longitude.HasValue)
            {
                return new CLLocationCoordinate2D(latitude.Value, longitude.Value);
            }

            return null;
        }

        private void SaveLocationSettings()
        {
            if (_locationManager != null &&
                _locationManager.Location != null)
            {
                _settings.AddOrUpdateValue(
                    LastLocationLat, 
                    _locationManager.Location.Coordinate.Latitude);
                _settings.AddOrUpdateValue(
                    LastLocationLong, 
                    _locationManager.Location.Coordinate.Longitude);
                _settings.Save();
            }
        }
    }
}