using System;
using Cirrious.MvvmCross.Binding.Touch.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Cirrious.MvvmCross.Binding.Binders;
using SmartWalk.Core.Utils;
using Cirrious.MvvmCross.Binding;
using SmartWalk.Core.Model;

namespace SmartWalk.iOS.Views.Cells
{
    public partial class OrgCell : MvxTableViewCell
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

        /*public string ImageUrl {
            get { return _imageHelper.ImageUrl; }
            set { _imageHelper.ImageUrl = value; }
        }*/

        public string ImageUrl {
            get { return null; }
            set { OrgImageView.Image = UIImage.FromFile(value); }
        }

        private void InitialiseImageHelper()
        {
            _imageHelper = new MvxImageViewLoader(
                () => OrgImageView);
        }
    }
}

