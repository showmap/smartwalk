using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace SmartWalk.Client.iOS.Controls
{
    [Register("TransparentToolBar")]
    public class TransparentToolBar : UIToolbar
    {
        public TransparentToolBar()
        {
        }

        public TransparentToolBar(IntPtr handle) : base(handle)
        {
        }

        public override void Draw(RectangleF rect)
        {
            // just cancel all drawings to keep it transparent
        }

        // TODO: To figure out how to hitTest map callout views
        public override UIView HitTest(PointF point, UIEvent uievent)
        {
            var view = base.HitTest(point, uievent);
            return view == this 
                ? Window.RootViewController.View.HitTest(point, uievent) 
                : view;
        }
    }
}