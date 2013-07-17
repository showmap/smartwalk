using System;
using Cirrious.MvvmCross.Binding;
using Cirrious.MvvmCross.Binding.Binders;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Core.Model;
using SmartWalk.Core.Utils;
using SmartWalk.Core.ViewModels;
using System.Drawing;

namespace SmartWalk.iOS.Views.Cells
{
    public partial class EntityCell : TableCellBase
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
            set
            { 
                if (value != null)
                {
                    LogoImageViewHeightConstraint.Constant = 240f;
                    LogoImageView.Image = UIImage.FromFile(value);
                }
                else
                {
                    LogoImageViewHeightConstraint.Constant = 0f;
                }
            }
        }

        public static float CalculateCellHeight(bool isExpanded, Entity entity)
        {
            var imageHeight = entity.Info.Logo != null ? 240f : 0f;

            if (isExpanded)
            {
                var isVertical = 
                    UIApplication.SharedApplication.StatusBarOrientation == UIInterfaceOrientation.Portrait || 
                        UIApplication.SharedApplication.StatusBarOrientation == UIInterfaceOrientation.PortraitUpsideDown;

                var frameSize = new SizeF(
                    (isVertical 
                         ? UIScreen.MainScreen.Bounds.Width 
                         : UIScreen.MainScreen.Bounds.Height),
                    float.MaxValue); // TODO:  - UIConstants.DefaultTextMargin * 2

                var textSize = new NSString(entity.Description).StringSize(
                    UIFont.FromName("Helvetica-Bold", 15),
                    frameSize,
                    UILineBreakMode.WordWrap);

                return textSize.Height + imageHeight + 30f + 10f;
            }
            else
            {
                return imageHeight + 30f + 70.0f + 10f;
            }
        }

        protected override bool Initialize()
        {
            var result = InitializeGestures();
            result = result && InitializeImageView();

            return result;
        }

        private void InitializeImageHelper()
        {
            /*_imageHelper = new MvxImageViewLoader(
                () => OrgImageView);*/
        }

        private bool InitializeImageView()
        {
            if (LogoImageView != null)
            {
                LogoImageView.BackgroundColor = UIColor.White;
                LogoImageView.ClipsToBounds = true;
                LogoImageView.Layer.BorderColor = UIColor.LightGray.CGColor;
                LogoImageView.Layer.BorderWidth = 1;
                LogoImageView.Layer.CornerRadius = 5;

                return true;
            }

            return false;
        }

        private bool InitializeGestures()
        {
            if (DescriptionLabel != null)
            {
                if (DescriptionLabel.GestureRecognizers == null ||
                    DescriptionLabel.GestureRecognizers.Length == 0)
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

                return true;
            }

            return false;
        }
    }
}