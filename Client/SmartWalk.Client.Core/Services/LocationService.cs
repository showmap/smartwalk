using System;

namespace SmartWalk.Client.Core.Services
{
    public class LocationService : ILocationService
    {
        public LocationService()
        {
            SetCurrentLocation();
        }

        public event EventHandler LocationChanged;

        public string CurrentLocation { get; private set; }

        private void SetCurrentLocation()
        {
            CurrentLocation = "us/ca/sfbay/";

            if (LocationChanged != null)
            {
                LocationChanged(this, EventArgs.Empty);
            }
        }
    }
}