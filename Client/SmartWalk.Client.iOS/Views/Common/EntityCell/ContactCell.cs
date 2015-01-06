using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Client.Core.Model.DataContracts;
using SmartWalk.Shared.DataContracts;
using SmartWalk.Client.iOS.Resources;
using SmartWalk.Client.iOS.Views.Common.Base.Cells;

namespace SmartWalk.Client.iOS.Views.Common.EntityCell
{
    public partial class ContactCell : CollectionCellBase
    {
        public static readonly UINib Nib = UINib.FromName("ContactCell", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("ContactCell");

        public const float DefaultHeight = 44;

        public ContactCell(IntPtr handle) : base (handle)
        {
        }

        public new Contact DataContext
        {
            get { return (Contact)base.DataContext; }
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
            ContactLabel.Text = DataContext != null ? DataContext.ContactText : null;

            ContactTopConstraint.Constant = DataContext != null && DataContext.Title != null ? 20 : 13;
            
            switch (DataContext.Type)
            {
                case ContactType.Phone:
                    ContactIcon.Image = ThemeIcons.ContactPhone;
                    break;

                case ContactType.Email:
                    ContactIcon.Image = ThemeIcons.ContactEmail;
                    break;

                case ContactType.Url:
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
            TitleLabel.TextColor = Theme.CellTextPassive;

            ContactLabel.Font = Theme.ContactTextFont;
            ContactLabel.TextColor = Theme.CellText;

            IconView.Color = Theme.IconActive;
        }
    }
}