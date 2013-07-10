using System;
using Cirrious.MvvmCross.Binding;
using Cirrious.MvvmCross.Binding.Binders;
using Cirrious.MvvmCross.Binding.Touch.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Core.Model;
using SmartWalk.Core.Utils;
using SmartWalk.Core.ViewModels;

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
                ReflectExtensions.GetPath<OrgViewModel, Org, EntityInfo>(p => p.Org, p => p.Info, p => p.Name), null, null, null, MvxBindingMode.OneWay),
            new MvxBindingDescription(
                Reflect<OrgCell>.GetProperty(p => p.OrgDescriptionText).Name,
                ReflectExtensions.GetPath<OrgViewModel, Org>(p => p.Org, p => p.Description), null, null, null, MvxBindingMode.OneWay),
            new MvxBindingDescription(
                Reflect<OrgCell>.GetProperty(p => p.ImageUrl).Name,
                ReflectExtensions.GetPath<OrgViewModel, Org, EntityInfo>(p => p.Org, p => p.Info, p => p.Logo), null, null, null, MvxBindingMode.OneWay)
        };

        //private MvxImageViewLoader _imageHelper;

        public OrgCell() : base(Bindings)
        {
            InitializeImageHelper();
        }

        public OrgCell(IntPtr handle) : base(Bindings, handle)
        {
            InitializeImageHelper();
        }

        public static OrgCell Create()
        {
            return (OrgCell)Nib.Instantiate(null, null)[0];
        }

        public OrgViewModel ViewModel
        {
            get { return (OrgViewModel)base.DataContext; }
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

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            InitGestures();
        }

        private void InitializeImageHelper()
        {
            /*_imageHelper = new MvxImageViewLoader(
                () => OrgImageView);*/
        }

        private void InitGestures()
        {
            if (OrgDescriptionLabel != null &&
                (OrgDescriptionLabel.GestureRecognizers == null ||
                OrgDescriptionLabel.GestureRecognizers.Length == 0))
            {
                var tap = new UITapGestureRecognizer(() => {
                    if (ViewModel.ExpandCollapseCommand.CanExecute(null))
                    {
                        ViewModel.ExpandCollapseCommand.Execute(null);
                    }
                });

                tap.NumberOfTouchesRequired = (uint)1;
                tap.NumberOfTapsRequired = (uint)1;

                OrgDescriptionLabel.AddGestureRecognizer(tap);
            }
        }
    }
}