using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace SmartWalk.Client.iOS.Utils
{
    public class UIBeingTouchedGestureRecognizer : UIGestureRecognizer
    {
        private Action<bool> _beingTouchedHandler;

        public UIBeingTouchedGestureRecognizer(Action<bool> beingTouchedHandler)
        {
            _beingTouchedHandler = beingTouchedHandler;
        }

        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            base.TouchesBegan(touches, evt);

            if (_beingTouchedHandler != null)
            {
                _beingTouchedHandler(true);
            }
        }

        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            base.TouchesEnded(touches, evt);

            if (_beingTouchedHandler != null)
            {
                _beingTouchedHandler(false);
            }
        }

        public override void TouchesCancelled(NSSet touches, UIEvent evt)
        {
            base.TouchesCancelled(touches, evt);

            if (_beingTouchedHandler != null)
            {
                _beingTouchedHandler(false);
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            _beingTouchedHandler = null;
        }
    }
}