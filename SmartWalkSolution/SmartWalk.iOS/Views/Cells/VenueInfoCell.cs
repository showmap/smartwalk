using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace SmartWalk.iOS.Views.Cells
{
    public partial class VenueInfoCell : UITableViewCell
    {
        public static readonly UINib Nib = UINib.FromName("VenueInfoCell", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("VenueInfoCell");

        public VenueInfoCell(IntPtr handle) : base (handle)
        {
        }

        public static VenueInfoCell Create()
        {
            return (VenueInfoCell)Nib.Instantiate(null, null)[0];
        }
    }
}