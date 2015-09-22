using System;
using Foundation;
using UIKit;
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

        public const float DefaultHeight = 50;

        public ContactCell(IntPtr handle) : base (handle)
        {
        }

        public new Contact DataContext
        {
            get { return (Contact)base.DataContext; }
            set { base.DataContext = value; }
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            InitializeStyle();
        }

        protected override void OnDataContextChanged()
        {
            TitleLabel.Text = DataContext != null ? DataContext.Title : null;
            ContactLabel.Text = DataContext != null ? DataContext.ContactText : null;

            ContactVerticalConstraint.Constant = DataContext != null && DataContext.Title != null ? -8 : 0;
            
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
            TitleLabel.TextColor = ThemeColors.ContentLightTextPassive;

            ContactLabel.Font = Theme.ContentFont;
            ContactLabel.TextColor = ThemeColors.ContentLightText;

            IconView.LineColor = ThemeColors.Action;
            ContactIcon.TintColor = ThemeColors.Action;
        }
    }
}