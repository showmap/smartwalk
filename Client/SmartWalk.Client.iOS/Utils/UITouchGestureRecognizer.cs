using System;
using Foundation;
using UIKit;

namespace SmartWalk.Client.iOS.Utils
{
    public class UITouchGestureRecognizer : UIGestureRecognizer
    {
        public UITouchGestureRecognizer(Action handler) : base(handler)
        {
        }

        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            base.TouchesBegan(touches, evt);

            State = UIGestureRecognizerState.Recognized;
        }
    }
}