using System;
using Cirrious.MvvmCross.Binding.Touch.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Client.Core.Model;
using SmartWalk.Client.iOS.Views.Common;

namespace SmartWalk.Client.iOS.Views.HomeView
{
    public partial class OrgCell : CollectionCellBase
    {
        public static readonly UINib Nib = UINib.FromName("OrgCell", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("OrgCell");

        private readonly MvxImageViewLoader _imageHelper;

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

        protected override void OnDataContextChanged()
        {
            OrgImageView.Image = null;

            _imageHelper.ImageUrl = DataContext != null ? DataContext.Logo : null;
            OrgNameLabel.Text = DataContext != null ? DataContext.Name : null;
        }
    }
}