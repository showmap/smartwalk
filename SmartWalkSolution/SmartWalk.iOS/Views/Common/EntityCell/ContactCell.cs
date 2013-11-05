using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Core.Model.Interfaces;
using SmartWalk.iOS.Resources;

namespace SmartWalk.iOS.Views.Common.EntityCell
{
    public partial class ContactCell : CollectionCellBase
    {
        public static readonly UINib Nib = UINib.FromName("ContactCell", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("ContactCell");

        public const float DefaultHeight = 50;

        public ContactCell(IntPtr handle) : base (handle)
        {
        }

        public new IContact DataContext
        {
            get { return (IContact)base.DataContext; }
            set { base.DataContext = value; }
        }

        public static ContactCell Create()
        {
            return (ContactCell)Nib.Instantiate(null, null)[0];
        }

        protected override void OnInitialize()
        {
            InitializeStyle();
        }

        protected override void OnDataContextChanged()
        {
            TitleLabel.Text = DataContext != null ? DataContext.Title : null;
            ContactLabel.Text = DataContext != null ? DataContext.Contact : null;

            TitleHeightConstraint.Constant = DataContext != null && DataContext.Title != null ? 22 : 0;
            ContactTopConstraint.Constant = DataContext != null && DataContext.Title != null ? 19 : 13;
            
            switch (DataContext.Type)
            {
                case ContactType.Phone:
                    ContactIcon.Image = ThemeIcons.ContactPhone;
                    break;

                case ContactType.Email:
                    ContactIcon.Image = ThemeIcons.ContactEmail;
                    break;

                case ContactType.WebSite:
                    ContactIcon.Image = ThemeIcons.ContactWeb;
                    break;

                default:
                    ContactIcon.Image = null;
                    break;
            }
        }

        private void InitializeStyle()
        {
            TitleLabel.Font = Theme.ContactTitleTextFont;
            TitleLabel.TextColor = Theme.HyperlinkText;

            ContactLabel.Font = Theme.ContactTextFont;
            ContactLabel.TextColor = Theme.CellText;
        }
    }
}