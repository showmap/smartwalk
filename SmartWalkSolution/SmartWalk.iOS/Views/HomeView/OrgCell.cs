using System;
using Cirrious.MvvmCross.Binding;
using Cirrious.MvvmCross.Binding.Binders;
using Cirrious.MvvmCross.Binding.Touch.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Core.Model;
using SmartWalk.Core.Utils;
using SmartWalk.iOS.Views.Common;

namespace SmartWalk.iOS.Views.HomeView
{
    public partial class OrgCell : TableCellBase
    {
        public static readonly UINib Nib = UINib.FromName("OrgCell", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("OrgCell");

        private static readonly MvxBindingDescription[] Bindings = new [] {
            new MvxBindingDescription(
                Reflect<EntityCell>.GetProperty(p => p.NameText).Name,
                Reflect<EntityInfo>.GetProperty(p => p.Name).Name,
                null, null, null, MvxBindingMode.OneWay),
            new MvxBindingDescription(
                Reflect<EntityCell>.GetProperty(p => p.ImageUrl).Name,
                Reflect<EntityInfo>.GetProperty(p => p.Logo).Name,
                null, null, null, MvxBindingMode.OneWay)
        };

        private MvxImageViewLoader _imageHelper;

        public OrgCell() : base(Bindings)
        {
            InitialiseImageHelper();      
        }

        public OrgCell(IntPtr handle) : base (Bindings, handle)
        {
            InitialiseImageHelper();
        }

        public static OrgCell Create()
        {
            return (OrgCell)Nib.Instantiate(null, null)[0];
        }

        public string NameText {
            get { return OrgNameLabel.Text; }
            set { OrgNameLabel.Text = value; }
        }

        public string ImageUrl {
            get { return _imageHelper.ImageUrl; }
            set { _imageHelper.ImageUrl = value; }
        }

        protected override bool Initialize()
        {
            var result = InitializeImageView();

            return result;
        }

        private bool InitializeImageView()
        {
            if (OrgImageView != null)
            {
                OrgImageView.BackgroundColor = UIColor.White;
                OrgImageView.ClipsToBounds = true;
                OrgImageView.Layer.BorderColor = UIColor.LightGray.CGColor;
                OrgImageView.Layer.BorderWidth = 1;
                OrgImageView.Layer.CornerRadius = 5;

                return true;
            }

            return false;
        }

        private void InitialiseImageHelper()
        {
            _imageHelper = new MvxImageViewLoader(() => OrgImageView);
        }
    }
}

