using System;
using MonoTouch.Foundation;

namespace SmartWalk.Core.Services
{
    public class LocationService : ILocationService
    {
        public LocationService()
        {
            NSTimer.CreateScheduledTimer(
                TimeSpan.MinValue,
                new NSAction(SetCurrentLocation));
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