using System;
using System.Drawing;
using Cirrious.MvvmCross.Binding;
using Cirrious.MvvmCross.Binding.Binders;
using Cirrious.MvvmCross.Binding.BindingContext;
using Cirrious.MvvmCross.Binding.Touch.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Core.Model;
using SmartWalk.Core.Utils;
using SmartWalk.Core.ViewModels;
using SmartWalk.iOS.Utils;

namespace SmartWalk.iOS.Views.Common.EntityCell
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
                    null, null, null, MvxBindingMode.OneWay),
            new MvxBindingDescription(
                Reflect<EntityCell>.GetProperty(p => p.Info).Name,
                ReflectExtensions.GetPath<EntityViewModel, Entity>(p => p.Entity, p => p.Info), 
                null, null, null, MvxBindingMode.OneWay),
        };

        private MvxImageViewLoader _imageHelper;

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
            set
            {
                DescriptionLabel.Text = value;
                DescriptionLabel.SizeToFit();
            }
        }

        public string ImageUrl {
            get { return _imageHelper.ImageUrl; }
            set
            { 
                if (value != null)
                {
                    ScrollViewHeightConstraint.Constant = 240f;
                    _imageHelper.ImageUrl = value;  // UIImage.FromFile(value);
                }
                else
                {
                    ScrollViewHeightConstraint.Constant = 0f;
                }
            }
        }

        public EntityInfo Info
        {
            get { return null; }
            set 
            {
                PageControl.Hidden = value != null ? !IsScrollViewVisible(value) : false;
            }
        }

        public override RectangleF Frame
        {
            set
            {
                base.Frame = value;

                if (ImageViewWidthConstraint != null)
                {
                    ImageViewWidthConstraint.Constant = ScreenUtil.CurrentScreenWidth;
                }

                if (ContactViewWidthConstraint != null)
                {
                    ContactViewWidthConstraint.Constant = ScreenUtil.CurrentScreenWidth;
                }

                if (ScrollView != null && ScrollView.Delegate != null)
                {
                    ((EntityScrollViewDelegate)ScrollView.Delegate).ScrollToCurrentPage();
                }
            }
        }

        // TODO: it's still buggy
        public static float CalculateCellHeight(bool isExpanded, Entity entity)
        {
            var nameLabelHeight = 30f + 5f;
            var scrollViewHeight = 240f;
            var pagerHeight = 20f;
            var bottomMargin = 8f;

            var isScrollVisible = IsScrollViewVisible(entity.Info);

            var cellHeight = nameLabelHeight + 
                (isScrollVisible ? scrollViewHeight : 0f) + 
                    pagerHeight + 
                    bottomMargin;

            if (isExpanded)
            {
                var textHeight = default(float);

                if (entity.Description != null)
                {
                    var frameSize = new SizeF(ScreenUtil.CurrentScreenWidth - 8 * 2, float.MaxValue); 
                    var textSize = new NSString(entity.Description).StringSize(
                        UIFont.FromName("Helvetica", 15),
                        frameSize,
                        UILineBreakMode.TailTruncation);
                    textHeight = textSize.Height + 25; // magic number
                }

                return cellHeight + textHeight;
            }
            else
            {
                return cellHeight + (entity.Description != null ? 63.0f : 0);
            }
        }

        private static bool IsScrollViewVisible(EntityInfo info)
        {
            return info.Logo != null || 
                (info.Contact != null && !info.Contact.IsEmpty);
        }

        protected override bool Initialize()
        {
            var result = InitializeGestures();
            result = result && InitializeImageView();
            result = result && InitializeGoToContactButton();
            result = result && InitializeScrollView();
            result = result && InitializeContactCollectionView();
            return result;
        }

        private void InitializeImageHelper()
        {
            _imageHelper = new MvxImageViewLoader(
                () => LogoImageView);
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

        private bool InitializeGoToContactButton()
        {
            if (GoToContactButton != null)
            {
                /*GoToContactButton.Layer.CornerRadius = 0;
                GoToContactButton.Layer.BorderWidth = 0;
                GoToContactButton.Layer.BackgroundColor = UIColor.LightGray.CGColor;*/

                return true;
            }

            return false;
        }

        private bool InitializeScrollView()
        {
            if (ScrollView != null)
            {
                if (ScrollView.Delegate == null)
                {
                    ScrollView.Delegate = new EntityScrollViewDelegate(ScrollView, PageControl);
                }

                return true;
            }

            return false;
        }

        private bool InitializeContactCollectionView()
        {
            if (ContactCollectionView != null)
            {
                if (ContactCollectionView.Source == null)
                {
                    var collectionSource = new ContactCollectionSource(ContactCollectionView);

                    this.CreateBinding(collectionSource)
                        .WithConversion(new ContactCollectionSourceConverter(), null)
                        .To((EntityViewModel vm) => vm.Entity.Info.Contact).Apply();

                    ContactCollectionView.Source = collectionSource;
                    ContactCollectionView.Delegate = new ContactCollectionDelegate();

                    ContactCollectionView.ReloadData();
                }

                return true;
            }

            return false;
        }

        partial void OnGoToContactButtonTouchUpInside(UIButton sender, UIEvent @event)
        {
            PageControl.CurrentPage = PageControl.CurrentPage == 0 ? 1 : 0;
            ((EntityScrollViewDelegate)ScrollView.Delegate).ScrollToCurrentPage();
        }
    }
}