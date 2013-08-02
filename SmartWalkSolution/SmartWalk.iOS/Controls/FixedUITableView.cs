using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace SmartWalk.iOS.Controls
{
    // HACK: taken from http://stackoverflow.com/questions/14307037/bug-in-uitableview-layout-after-orientation-change
    [Register("FixedUITableView")]
    public class FixedUITableView : UITableView
    {
        public FixedUITableView(IntPtr handle) : base(handle)
        {
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            var contentSize = ContentSize;
            contentSize.Width  = Bounds.Size.Width;
            ContentSize = contentSize;
        }
    }
}