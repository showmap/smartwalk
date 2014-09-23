using System;
using System.Linq;
using System.Threading.Tasks;
using MonoTouch.CoreLocation;
using MonoTouch.UIKit;
using Refractored.MvxPlugins.Settings;
using SmartWalk.Client.Core.Model;
using SmartWalk.Client.Core.Resources;
using SmartWalk.Client.Core.Services;
using SmartWalk.Client.Core.Utils;

namespace SmartWalk.Client.iOS.Services
{
    public class LocationService : ILocationService
    {
        private const string LastLocationLat = "LastLocationLat";
        private const string LastLocationLong = "LastLocationLong";

        private readonly ISettings _settings;
        private readonly IExceptionPolicyService _exceptionPolicy;
        private readonly IEnvironmentService _environmentService;

        private CLLocationManager _locationManager;
        private CLGeocoder _geocoder;

        private bool _isActive;
        private bool _isMonitoring;

        public LocationService(
            ISettings settings, 
            IExceptionPolicyService exceptionPolicy,
            IEnvironmentService environmentService)
        {
            _settings = settings;
            _exceptionPolicy = exceptionPolicy;
            _environmentService = environmentService;

            if (CLLocationManager.LocationServicesEnabled)
            {
                Initialize();
            }

            SaveLocationSettings();
            UpdateLocationString().ContinueWithThrow();
        }

        public event EventHandler LocationChanged;
        public event EventHandler LocationStringChanged;

        public bool IsActive
        {
            get
            {
                return _isActive;
            }

            set
            {
                if (_isActive != value)
                {
                    _isActive = value;

                    if (_isActive)
                    {
                        OnActivate();
                    }
                    else
                    {
                        OnDeactivate();
                    }
                }
            }
        }

        public Location CurrentLocation 
        { 
            get
            { 
                CLLocationCoordinate2D? coordinate;

                if (IsLocationAvailable)
                {
                    coordinate = _locationManager.Location.Coordinate;
                }
                else
                {
                    coordinate = LoadLocationSettings();
                }

                if (coordinate.HasValue)
                {
                    // using bigger region to filter nearby events
                    var result = new Location(
                        Math.Round(coordinate.Value.Latitude, 2),
                        Math.Round(coordinate.Value.Longitude, 2));
                    return result;
                }

                return Location.Empty;
            }
        }

        public string CurrentLocationString { get; private set; }

        private bool IsLocationAccessible
        {
            get
            {
                return 
                    _locationManager != null &&
                    (CLLocationManager.Status == CLAuthorizationStatus.AuthorizedAlways ||
                        CLLocationManager.Status == CLAuthorizationStatus.AuthorizedWhenInUse);
            }
        }

        private bool IsLocationAvailable
        {
            get
            {
                return IsLocationAccessible && _locationManager.Location != null;
            }
        }

        public void ResolveLocationIssues()
        {
            if (IsLocationAccessible)
            {
                if (CurrentLocation == Location.Empty)
                {
                    // if accessible but no location, maybe monitoring was turned off
                    StopMonitoring();
                    StartMonitoring();
                }
            }
            else
            {
                if (!CLLocationManager.LocationServicesEnabled)
                {
                    _environmentService.Alert(Localization.LocationError, Localization.CantResolveLocationServicesOff);
                }
                else if (CLLocationManager.Status == CLAuthorizationStatus.Denied ||
                         CLLocationManager.Status == CLAuthorizationStatus.Restricted)
                {
                    _environmentService.Alert(Localization.LocationError, Localization.CantResolveLocationDenied);
                }
                else
                {
                    Initialize();
                    StopMonitoring();
                    StartMonitoring();
                }
            }
        }

        private void Initialize()
        {
            if (_geocoder == null)
            {
                _geocoder = new CLGeocoder();
            }

            if (_locationManager == null)
            {
                _locationManager = new CLLocationManager();
                _locationManager.DesiredAccuracy = CLLocation.AccuracyThreeKilometers;
                _locationManager.AuthorizationChanged += OnAuthorizationChanged;
            }
        }

        private void OnActivate()
        {
            StartMonitoring();
        }

        private void OnDeactivate()
        {
            StopMonitoring();
        }

        private void StartMonitoring()
        {
            if (_locationManager != null && !_isMonitoring)
            {
                var monitoringAllowed = false;

                switch (CLLocationManager.Status)
                {
                    case CLAuthorizationStatus.AuthorizedAlways: 
                    case CLAuthorizationStatus.AuthorizedWhenInUse:
                        monitoringAllowed = true;
                        break;

                    case CLAuthorizationStatus.NotDetermined:
                        if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
                        {
                            _locationManager.RequestWhenInUseAuthorization();
                        }
                        else
                        {
                            monitoringAllowed = true;
                        }
                        break;
                }

                if (monitoringAllowed)
                {
                    _locationManager.LocationsUpdated += OnLocationsUpdated;

                    if (CLLocationManager.SignificantLocationChangeMonitoringAvailable)
                    {
                        _locationManager.StartMonitoringSignificantLocationChanges();
                    }
                    else
                    {
                        _locationManager.DistanceFilter = 3000; // 3 Km
                        _locationManager.StartUpdatingLocation();
                    }

                    _isMonitoring = true;
                }
            }
        }

        private void StopMonitoring()
        {
            if (_locationManager != null && _isMonitoring)
            {
                _locationManager.StopMonitoringSignificantLocationChanges();
                _locationManager.StopUpdatingLocation();
                _locationManager.DistanceFilter = CLLocationDistance.FilterNone;
                _locationManager.LocationsUpdated -= OnLocationsUpdated;
                _isMonitoring = false;
            }
        }

        private void OnAuthorizationChanged(object sender, CLAuthorizationChangedEventArgs e)
        {
            if (IsActive)
            {
                if (e.Status == CLAuthorizationStatus.AuthorizedAlways ||
                    e.Status == CLAuthorizationStatus.AuthorizedWhenInUse)
                {
                    StartMonitoring();
                }
                else
                {
                    StopMonitoring();
                }
            }
        }

        private void OnLocationsUpdated(object sender, CLLocationsUpdatedEventArgs e)
        {
            var lastLocation = LoadLocationSettings();
            var location = _locationManager.Location == null 
                ? (CLLocationCoordinate2D?)null 
                : _locationManager.Location.Coordinate;

            if (!Equals(lastLocation, location))
            {
                SaveLocationSettings();

                if (LocationChanged != null)
                {
                    LocationChanged(this, EventArgs.Empty);
                }
            }

            UpdateLocationString().ContinueWithThrow();
        }

        private async Task UpdateLocationString()
        {
            if (_geocoder != null && CurrentLocation != Location.Empty)
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
                    if (placemark != null &&
                        placemark.Locality != null &&
                        placemark.Country != null)
                    {
                        CurrentLocationString = string.Format(
                            "{0}, {1}", 
                            placemark.Locality, 
                            placemark.Country);
                    }
                    else
                    {
                        CurrentLocationString = Localization.UnknownLocation;
                    }
                }
            }
            else
            {
                CurrentLocationString = Localization.UnknownLocation;
            }

            if (LocationStringChanged != null)
            {
                LocationStringChanged(this, EventArgs.Empty);
            }
        }

        private CLLocationCoordinate2D? LoadLocationSettings()
        {
            if (IsLocationAccessible)
            {
                var latitude = _settings.GetValueOrDefault<double?>(LastLocationLat);
                var longitude = _settings.GetValueOrDefault<double?>(LastLocationLong);

                if (latitude.HasValue && longitude.HasValue)
                {
                    return new CLLocationCoordinate2D(latitude.Value, longitude.Value);
                }
            }

            return null;
        }

        private void SaveLocationSettings()
        {
            if (IsLocationAvailable)
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