using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Core.Model;
using SmartWalk.iOS.Resources;

namespace SmartWalk.iOS.Views.Common.EntityCell
{
    public partial class EmailCell : CollectionCellBase
    {
        public static readonly UINib Nib = UINib.FromName("EmailCell", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("EmailCell");

        public EmailCell(IntPtr handle) : base(handle)
        {
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

        protected override void OnInitialize()
        {
            ContactNameLabel.TextColor = Theme.HyperlinkText;
            LogoImageView.Image = ThemeIcons.ContactEmail;
        }

        protected override void OnDataContextChanged()
        {
            ContactNameLabel.Text = DataContext != null ? DataContext.Name : null;
            EmailLabel.Text = DataContext != null ? DataContext.Email : null;
            NameHeightConstraint.Constant = DataContext != null && DataContext.Name != null ? 20 : 0;
        }
    }
}