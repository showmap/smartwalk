using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace SmartWalk.iOS.Views.Cells
{
    public partial class VenueShowInfoCell : UITableViewCell
    {
        public static readonly UINib Nib = UINib.FromName("VenueShowInfoCell", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("VenueShowInfoCell");

        public VenueShowInfoCell(IntPtr handle) : base (handle)
        {
        }

        public static VenueShowInfoCell Create()
        {
            return (VenueShowInfoCell)Nib.Instantiate(null, null)[0];
        }
    }
}

