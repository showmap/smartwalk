using System;
using System.Linq;
using System.Threading.Tasks;
using CoreLocation;
using UIKit;
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
        private readonly IReachabilityService _reachabilityService;
        private readonly IEnvironmentService _environmentService;

        private readonly CLLocationManager _locationManager;
        private readonly CLGeocoder _geocoder;

        private bool _isActive;
        private bool _isMonitoring;
        private Location? _lastLocation;
        private string _currentLocationString;

        public LocationService(
            ISettings settings, 
            IExceptionPolicyService exceptionPolicy,
            IReachabilityService reachabilityService,
            IEnvironmentService environmentService)
        {
            _settings = settings;
            _exceptionPolicy = exceptionPolicy;
            _environmentService = environmentService;
            _reachabilityService = reachabilityService;
            _reachabilityService.StateChanged += 
                (s, e) => UpdateLocationString().ContinueWithThrow();

            _geocoder = new CLGeocoder();
            _locationManager = new CLLocationManager();
            _locationManager.DesiredAccuracy = CLLocation.AccuracyThreeKilometers;
            _locationManager.DistanceFilter = 3000; // 3 Km
            _locationManager.AuthorizationChanged += OnAuthorizationChanged;

            SaveLocationSettings(CurrentLocation);
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

        /// <summary>
        /// Gets the current location if Loc Services are available or 
        /// a last saved one, if they aren't.
        /// </summary>
        public Location CurrentLocation
        { 
            get
            { 
                var location = IsLocationAvailable 
                    ? GetLocationByCoordinate(_locationManager.Location.Coordinate) 
                    : LoadLocationSettings();

                return location.HasValue ? location.Value : Location.Empty;

            }
        }

        /// <summary>
        /// Gets the current location string if there is Network Connection and Loc Services are available.
        /// Otherwise null is returned.
        /// </summary>
        public string CurrentLocationString
        {
            get
            {
                return _currentLocationString;
            }
            private set
            {
                if (_currentLocationString != value)
                {
                    _currentLocationString = value;

                    if (LocationStringChanged != null)
                    {
                        LocationStringChanged(this, EventArgs.Empty);
                    }
                }
            }
        }

        private static bool IsLocationAccessible
        {
            get
            {
                return 
                    CLLocationManager.LocationServicesEnabled && 
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
            if (!IsActive) return;

            if (IsLocationAccessible)
            {
                if (CurrentLocationString == null)
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
                    _environmentService.Alert(
                        Localization.LocationError,
                        Localization.CantResolveLocationServicesOff);
                }
                else if (CLLocationManager.Status == CLAuthorizationStatus.Denied ||
                         CLLocationManager.Status == CLAuthorizationStatus.Restricted)
                {
                    _environmentService.Alert(
                        Localization.LocationError, 
                        Localization.CantResolveLocationDenied);
                }
                else
                {
                    StopMonitoring();
                    StartMonitoring();
                }
            }
        }

        public void RefreshLocation()
        {
            if (IsActive)
            {
                StartMonitoring();
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
            if (!_isMonitoring)
            {
                var monitoringAllowed = false;

                if (CLLocationManager.LocationServicesEnabled)
                {
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
                }

                if (monitoringAllowed)
                {
                    _locationManager.LocationsUpdated += OnLocationsUpdated;
                    _locationManager.StartUpdatingLocation();

                    _isMonitoring = true;
                }
                else
                {
                    // Reseting location string to null
                    UpdateLocationString().ContinueWithThrow();
                }
            }
        }

        private void StopMonitoring()
        {
            if (_isMonitoring)
            {
                _locationManager.StopUpdatingLocation();
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
            // we just need a one-time location at current moment
            StopMonitoring();

            var location = _locationManager.Location == null 
                ? (Location?)null 
                : GetLocationByCoordinate(_locationManager.Location.Coordinate);

            if (!Equals(_lastLocation, location))
            {
                _lastLocation = location;

                SaveLocationSettings(location);
                UpdateLocationString().ContinueWithThrow();

                if (LocationChanged != null)
                {
                    LocationChanged(this, EventArgs.Empty);
                }
            }
        }

        private async Task UpdateLocationString()
        {
            var result = default(string);

            if (IsLocationAccessible && CurrentLocation != Location.Empty)
            {
                var isConnected = await _reachabilityService.GetIsReachable();
                if (isConnected)
                {
                    var placemarks = default(CLPlacemark[]);

                    try
                    {
                        var location = 
                            new CLLocation(
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
                            placemark.Locality != null)
                        {
                            result = placemark.Locality;
                        }
                    }
                }
            }

            CurrentLocationString = result;
        }

        private Location? LoadLocationSettings()
        {
            var latitude = _settings.GetValueOrDefault<double?>(LastLocationLat);
            var longitude = _settings.GetValueOrDefault<double?>(LastLocationLong);

            if (latitude.HasValue && longitude.HasValue)
            {
                return new Location(latitude.Value, longitude.Value);
            }

            return null;
        }

        private void SaveLocationSettings(Location? location)
        {
            if (IsLocationAccessible && 
                location != null && location != Location.Empty)
            {
                _settings.AddOrUpdateValue(LastLocationLat, location.Value.Latitude);
                _settings.AddOrUpdateValue(LastLocationLong, location.Value.Longitude);
                _settings.Save();
            }
        }

        /// <summary>
        /// Gets the SW location by coordinate with smaller precision to filter nearby events.
        /// </summary>
        private static Location GetLocationByCoordinate(CLLocationCoordinate2D coordinate)
        {
            var result = new Location(
                Math.Round(coordinate.Latitude, 2),
                Math.Round(coordinate.Longitude, 2));
            return result;
        }

        private static CLLocationCoordinate2D GetCoordinateByLocation(Location location)
        {
            var result = new CLLocationCoordinate2D(location.Latitude, location.Longitude);
            return result;
        }
    }
}