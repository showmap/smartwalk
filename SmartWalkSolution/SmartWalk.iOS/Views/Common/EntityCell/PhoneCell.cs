using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Core.Model;

namespace SmartWalk.iOS.Views.Common.EntityCell
{
    public partial class PhoneCell : CollectionCellBase
    {
        public static readonly UINib Nib = UINib.FromName("PhoneCell", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("PhoneCell");

        public PhoneCell(IntPtr handle) : base(handle)
        {
            //Layer.BorderColor = UIColor.Gray.CGColor;
            //Layer.BorderWidth = 1;
            Layer.CornerRadius = 3;
        }

        public new PhoneInfo DataContext
        {
            get { return (PhoneInfo)base.DataContext; }
            set { base.DataContext = value; }
        }

        public static PhoneCell Create()
        {
            return (PhoneCell)Nib.Instantiate(null, null)[0];
        }

        protected override void OnDataContextChanged()
        {
            ContactNameLabel.Text = DataContext != null ? DataContext.Name : null;
            PhoneNumberLabel.Text = DataContext != null ? DataContext.Phone : null;
            NameHeightConstraint.Constant = DataContext != null && DataContext.Name != null ? 20 : 0;
        }
    }
}