using System;
using System.Windows.Input;
using Cirrious.MvvmCross.Binding;
using Cirrious.MvvmCross.Binding.Binders;
using Cirrious.MvvmCross.Binding.Touch.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Core.Model;
using SmartWalk.Core.Utils;

namespace SmartWalk.iOS.Views.Cells
{
    public partial class OrgCell : MvxTableViewCell
    {
        public static readonly UINib Nib = UINib.FromName("OrgCell",
            NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("OrgCell");

        private static MvxBindingDescription[] Bindings = new [] {
            new MvxBindingDescription(
                Reflect<OrgCell>.GetProperty(p => p.OrgNameText).Name,
                ReflectExtensions.GetPath<Org, EntityInfo>(p => p.Info, p => p.Name), null, null, null, MvxBindingMode.OneWay),
            new MvxBindingDescription(
                Reflect<OrgCell>.GetProperty(p => p.OrgDescriptionText).Name,
                ReflectExtensions.GetPath<Org>(p => p.Description), null, null, null, MvxBindingMode.OneWay),
            new MvxBindingDescription(
                Reflect<OrgCell>.GetProperty(p => p.ImageUrl).Name,
                ReflectExtensions.GetPath<Org, EntityInfo>(p => p.Info, p => p.Logo), null, null, null, MvxBindingMode.OneWay)
        };

        //private MvxImageViewLoader _imageHelper;

        public OrgCell() : base(Bindings)
        {
            InitialiseImageHelper();      
        }

        public OrgCell(IntPtr handle) : base(Bindings, handle)
        {
            InitialiseImageHelper();
        }

        public static OrgCell Create()
        {
            return (OrgCell)Nib.Instantiate(null, null)[0];
        }

        public string OrgNameText {
            get { return OrgNameLabel.Text; }
            set { OrgNameLabel.Text = value; }
        }

        public string OrgDescriptionText {
            get { return OrgDescriptionLabel.Text; }
            set { OrgDescriptionLabel.Text = value; }
        }

        /*public string ImageUrl {
            get { return _imageHelper.ImageUrl; }
            set { _imageHelper.ImageUrl = value; }
        }*/

        public string ImageUrl {
            get { return null; }
            set { OrgImageView.Image = UIImage.FromFile(value); }
        }

        public ICommand ExpandCollapseCommand { get; set; }

        private void InitialiseImageHelper()
        {
            /*_imageHelper = new MvxImageViewLoader(
                () => OrgImageView);*/
        }

        partial void OnExpandCollapseButtonClick(UIButton sender)
        {
            if (ExpandCollapseCommand != null &&
                ExpandCollapseCommand.CanExecute(null))
            {
                ExpandCollapseCommand.Execute(null);
            }
        }
    }
}