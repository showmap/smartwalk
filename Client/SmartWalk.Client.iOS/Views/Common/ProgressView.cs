using System;
using MonoTouch.UIKit;
using SmartWalk.Client.Core.Utils;
using MonoTouch.Foundation;

namespace SmartWalk.Client.iOS.Views.Common
{
    public partial class ProgressView : UIView
    {
        public static readonly UINib Nib = UINib.FromName("ProgressView", NSBundle.MainBundle);

        public ProgressView(IntPtr handle) : base(handle)
        {
        }

        public static ProgressView Create()
        {
            return (ProgressView)Nib.Instantiate(null, null)[0];
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            ConsoleUtil.LogDisposed(this);
        }
    }
}