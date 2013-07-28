using System;
using Cirrious.MvvmCross.Binding.Touch.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Core.Model;
using SmartWalk.iOS.Views.Common;

namespace SmartWalk.iOS.Views.HomeView
{
    public partial class OrgCell : CollectionCellBase<EntityInfo>
    {
        public static readonly UINib Nib = UINib.FromName("OrgCell", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("OrgCell");

        private MvxImageViewLoader _imageHelper;

        public OrgCell(IntPtr handle) : base(handle)
        {
            _imageHelper = new MvxImageViewLoader(() => OrgImageView);

            //Layer.BorderColor = UIColor.Gray.CGColor;
            //Layer.BorderWidth = 1;
            Layer.CornerRadius = 5;
        }

        public static OrgCell Create()
        {
            return (OrgCell)Nib.Instantiate(null, null)[0];
        }

        protected override void OnInitialize()
        {
            InitializeImageView();
        }

        protected override void OnDataContextChanged()
        {
            _imageHelper.ImageUrl = DataContext != null ? DataContext.Logo : null;
            OrgNameLabel.Text = DataContext != null ? DataContext.Name : null;
        }

        private void InitializeImageView()
        {
            OrgImageView.BackgroundColor = UIColor.White;
            OrgImageView.ClipsToBounds = true;
            //OrgImageView.Layer.BorderColor = UIColor.LightGray.CGColor;
            //OrgImageView.Layer.BorderWidth = 1;
            OrgImageView.Layer.CornerRadius = 3;
        }
    }
}

