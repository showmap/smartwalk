using System;
using Cirrious.MvvmCross.Binding.Touch.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Core.Model;
using SmartWalk.iOS.Resources;
using SmartWalk.iOS.Views.Common;

namespace SmartWalk.iOS.Views.HomeView
{
    public partial class OrgCell : CollectionCellBase
    {
        public static readonly UINib Nib = UINib.FromName("OrgCell", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("OrgCell");

        private MvxImageViewLoader _imageHelper;

        public OrgCell(IntPtr handle) : base(handle)
        {
            _imageHelper = new MvxImageViewLoader(() => OrgImageView);
        }

        public new EntityInfo DataContext
        {
            get { return (EntityInfo)base.DataContext; }
            set { base.DataContext = value; }
        }

        public static OrgCell Create()
        {
            return (OrgCell)Nib.Instantiate(null, null)[0];
        }

        public void SetSelected(bool isSelected)
        {
            SetAttributedString(isSelected);
        }

        protected override void OnDataContextChanged()
        {
            OrgImageView.Image = null;

            _imageHelper.ImageUrl = DataContext != null ? DataContext.Logo : null;

            SetAttributedString();
        }

        private void SetAttributedString(bool isSelected = false)
        {
            var result = default(NSAttributedString);

            if (DataContext != null && DataContext.Name != null)
            {
                var paragraphStyle = new NSMutableParagraphStyle { 
                    LineSpacing = Theme.OrgTextLineSpacing
                };

                result = new NSAttributedString(
                    DataContext.Name, 
                    null, 
                    isSelected ? OrgNameLabel.HighlightedTextColor: null, 
                    null, 
                    null, 
                    paragraphStyle);
            }

            OrgNameLabel.AttributedText = result;
        }
    }
}