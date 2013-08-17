using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Core.Model;

namespace SmartWalk.iOS.Views.Common.EntityCell
{
    public partial class EmailCell : CollectionCellBase
    {
        public static readonly UINib Nib = UINib.FromName("EmailCell", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("EmailCell");

        public EmailCell(IntPtr handle) : base(handle)
        {
            //Layer.BorderColor = UIColor.Gray.CGColor;
            //Layer.BorderWidth = 1;
            Layer.CornerRadius = 3;
        }

        public new EmailInfo DataContext
        {
            get { return (EmailInfo)base.DataContext; }
            set { base.DataContext = value; }
        }

        public static EmailCell Create()
        {
            return (EmailCell)Nib.Instantiate(null, null)[0];
        }

        protected override void OnDataContextChanged()
        {
            ContactNameLabel.Text = DataContext != null ? DataContext.Name : null;
            EmailLabel.Text = DataContext != null ? DataContext.Email : null;
            NameHeightConstraint.Constant = DataContext != null && DataContext.Name != null ? 20 : 0;
        }
    }
}