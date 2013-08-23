using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace SmartWalk.iOS.Views.Common
{
    public partial class ProgressCell : UITableViewCell
    {
        public static readonly UINib Nib = UINib.FromName("ProgressCell", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("ProgressCell");

        public ProgressCell(IntPtr handle) : base (handle)
        {
        }

        public static ProgressCell Create()
        {
            return (ProgressCell)Nib.Instantiate(null, null)[0];
        }
    }
}

