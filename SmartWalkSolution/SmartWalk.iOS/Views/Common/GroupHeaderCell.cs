using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace SmartWalk.iOS.Views.Common
{
    public partial class GroupHeaderCell : UITableViewHeaderFooterView
    {
        public static readonly UINib Nib = UINib.FromName("GroupHeaderCell", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("GroupHeaderCell");

        public GroupHeaderCell(IntPtr handle) : base(handle)
        {
            BackgroundView = new UIView();
        }

        public static GroupHeaderCell Create()
        {
            return (GroupHeaderCell)Nib.Instantiate(null, null)[0];
        }

        public string Text
        {
            get { return TitleLabel.Text; }
            set { TitleLabel.Text = value; }
        }
    }
}