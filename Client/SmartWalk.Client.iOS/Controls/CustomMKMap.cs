using System;
using MonoTouch.Foundation;
using MonoTouch.MapKit;
using MonoTouch.UIKit;

namespace SmartWalk.Client.iOS.Controls
{
    [Register("CustomMKMapView")]
    public class CustomMKMapView : MKMapView
    {
        public CustomMKMapView(IntPtr handle) : base(handle)
        {
        }

        public bool IsBeingTouched { get; private set; }

        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            base.TouchesBegan(touches, evt);

            IsBeingTouched = true;
        }

        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            base.TouchesEnded(touches, evt);

            IsBeingTouched = false;
        }

        public override void TouchesCancelled(NSSet touches, UIEvent evt)
        {
            base.TouchesCancelled(touches, evt);

            IsBeingTouched = false;
        }
    }
}