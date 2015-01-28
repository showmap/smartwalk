using System;
using CoreGraphics;
using Foundation;
using UIKit;
using SmartWalk.Client.iOS.Utils;

namespace SmartWalk.Client.iOS.Controls
{
    // HACK: taken from http://stackoverflow.com/questions/14307037/bug-in-uitableview-layout-after-orientation-change
    [Register("FixedTableView")]
    public class FixedTableView : UITableView
    {
        public FixedTableView(IntPtr handle) : base(handle)
        {
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            if (ContentSize != CGSize.Empty)
            {
                var contentSize = ContentSize;
                contentSize.Width = Bounds.Size.Width;
                ContentSize = contentSize;
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            ConsoleUtil.LogDisposed(this);
        }
    }
}