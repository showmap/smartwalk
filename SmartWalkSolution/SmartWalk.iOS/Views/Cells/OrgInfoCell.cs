using System;
using System.Drawing;
using Cirrious.MvvmCross.Binding.Touch.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace SmartWalk.iOS.Views.Cells
{
    public partial class OrgInfoCell : MvxTableViewCell
    {
        public static readonly UINib Nib = UINib.FromName("OrgInfoCell",
            NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("OrgInfoCell");

        public const string BindingText = @"
NameText Name;
ImageUrl Logo";

        private MvxImageViewLoader _imageHelper;

        public OrgInfoCell() : base(BindingText)
        {
            InitialiseImageHelper();      
        }

        public OrgInfoCell(IntPtr handle) : base (BindingText, handle)
        {
            InitialiseImageHelper();
        }

        public static OrgInfoCell Create()
        {
            return (OrgInfoCell)Nib.Instantiate(null, null)[0];
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

