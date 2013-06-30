using System;
using Cirrious.MvvmCross.Binding.Touch.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace SmartWalk.iOS.Views.Cells
{
    public partial class OrgCell : MvxTableViewCell
    {
        public static readonly UINib Nib = UINib.FromName("OrgCell",
            NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("OrgCell");

        public const string BindingText = @"
NameText Name;
ImageUrl Logo";

        private MvxImageViewLoader _imageHelper;

        public OrgCell() : base(BindingText)
        {
            InitialiseImageHelper();      
        }

        public OrgCell(IntPtr handle) : base (BindingText, handle)
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