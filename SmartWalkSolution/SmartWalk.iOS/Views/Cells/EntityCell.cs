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
    public partial class EntityCell : MvxTableViewCell
    {
        public static readonly UINib Nib = UINib.FromName("EntityCell", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("EntityCell");

        private static readonly MvxBindingDescription[] Bindings = new [] {
            new MvxBindingDescription(
                Reflect<EntityCell>.GetProperty(p => p.NameText).Name,
                ReflectExtensions.GetPath<EntityViewModel, Entity, EntityInfo>(p => p.Entity, p => p.Info, p => p.Name), 
                    null, null, null, MvxBindingMode.OneWay),
            new MvxBindingDescription(
                Reflect<EntityCell>.GetProperty(p => p.DescriptionText).Name,
                ReflectExtensions.GetPath<EntityViewModel, Entity>(p => p.Entity, p => p.Description), 
                    null, null, null, MvxBindingMode.OneWay),
            new MvxBindingDescription(
                Reflect<EntityCell>.GetProperty(p => p.ImageUrl).Name,
                ReflectExtensions.GetPath<EntityViewModel, Entity, EntityInfo>(p => p.Entity, p => p.Info, p => p.Logo), 
                    null, null, null, MvxBindingMode.OneWay)
        };

        public EntityCell() : base(Bindings)
        {
            InitializeImageHelper();      
        }

        public EntityCell(IntPtr handle) : base(Bindings, handle)
        {
            InitializeImageHelper();
        }

        public static EntityCell Create()
        {
            return (EntityCell)Nib.Instantiate(null, null)[0];
        }

        public EntityViewModel ViewModel
        {
            get { return (EntityViewModel)base.DataContext; }
        }

        public string NameText {
            get { return NameLabel.Text; }
            set { NameLabel.Text = value; }
        }

        public string DescriptionText {
            get { return DescriptionLabel.Text; }
            set { DescriptionLabel.Text = value; }
        }

        /*public string ImageUrl {
            get { return _imageHelper.ImageUrl; }
            set { _imageHelper.ImageUrl = value; }
        }*/

        public string ImageUrl {
            get { return null; }
            set { LogoImageView.Image = UIImage.FromFile(value); }
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
            if (DescriptionLabel != null &&
                (DescriptionLabel.GestureRecognizers == null ||
                    DescriptionLabel.GestureRecognizers.Length == 0))
            {
                var tap = new UITapGestureRecognizer(() => {
                    if (ViewModel.ExpandCollapseCommand.CanExecute(null))
                    {
                        ViewModel.ExpandCollapseCommand.Execute(null);
                    }
                });

                tap.NumberOfTouchesRequired = (uint)1;
                tap.NumberOfTapsRequired = (uint)1;

                DescriptionLabel.AddGestureRecognizer(tap);
            }
        }
    }
}