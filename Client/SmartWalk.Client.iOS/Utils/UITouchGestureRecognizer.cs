using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace SmartWalk.Client.iOS.Utils
{
    public class UITouchGestureRecognizer : UIGestureRecognizer
    {
        public UITouchGestureRecognizer(NSAction handler) : base(handler)
        {
        }

        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            base.TouchesBegan(touches, evt);

            State = UIGestureRecognizerState.Recognized;
        }
    }
}