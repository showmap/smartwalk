using System;
using System.Drawing;
using Cirrious.MvvmCross.Binding.BindingContext;
using Cirrious.MvvmCross.Binding.Touch.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Core.Model;
using SmartWalk.Core.ViewModels;
using SmartWalk.iOS.Utils;
using SmartWalk.Core.Converters;

namespace SmartWalk.iOS.Views.Common.EntityCell
{
    public partial class EntityCell : TableCellBase
    {
        public static readonly UINib Nib = UINib.FromName("EntityCell", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("EntityCell");

        private readonly MvxImageViewLoader _imageHelper;

        private UITapGestureRecognizer _descriptionTapGesture;
        private bool _isDescriptionTapGestureEnabled = true;

        public EntityCell(IntPtr handle) : base(handle)
        {
            _imageHelper = new MvxImageViewLoader(() => LogoImageView);

            this.DelayBind(() => {
                var set = this.CreateBindingSet<EntityCell, EntityViewModel>();
                set.Bind(NameLabel).To(vm => vm.Entity.Info.Name);
                set.Bind(DescriptionLabel).To(vm => vm.Entity.Description);
                set.Bind(_imageHelper).To(vm => vm.Entity.Info.Logo);
                set.Bind().For(cell => cell.IsDescriptionTapGestureEnabled)
                    .To(vm => vm.IsDescriptionExpandable);

                set.Bind(ScrollViewHeightConstraint).For(p => p.Constant).To(vm => vm.Entity.Info)
                    .WithConversion(new ValueConverter<EntityInfo>(
                        info => info != null && IsScrollViewVisible(info) ? 240 : 0), null);

                set.Bind(PageControl).For(p => p.Hidden).To(vm => vm.Entity.Info)
                    .WithConversion(new ValueConverter<EntityInfo>(
                        info => info != null ? !IsScrollViewVisible(info) : false), null);

                set.Apply();
            });
        }

        public static EntityCell Create()
        {
            return (EntityCell)Nib.Instantiate(null, null)[0];
        }

        public EntityViewModel ViewModel
        {
            get { return (EntityViewModel)base.DataContext; }
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

                if (ScrollView != null && PageControl != null)
                {
                    //HACK: to fix the page position
                    //TODO: to review EntityScrollViewDelegate.Scrolled() impl
                    ScrollView.ContentOffset = new PointF(
                        ScreenUtil.CurrentScreenWidth * PageControl.CurrentPage, 
                        0);
                }

                if (ContactCollectionView != null)
                {
                    SetCollectionViewCellWidth();
                }
            }
        }

        public bool IsDescriptionTapGestureEnabled
        {
            get { return _isDescriptionTapGestureEnabled; }
            set 
            { 
                _isDescriptionTapGestureEnabled = value;

                if (_descriptionTapGesture != null)
                {
                    _descriptionTapGesture.Enabled = _isDescriptionTapGestureEnabled;
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

                if (entity.Description != null && entity.Description != string.Empty)
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

            return cellHeight + 
                (entity.Description != null && entity.Description != string.Empty ? 63.0f : 0);
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

        private bool InitializeImageView()
        {
            if (LogoImageView != null)
            {
                //LogoImageView.BackgroundColor = UIColor.White;
                //LogoImageView.ClipsToBounds = true;
                //LogoImageView.Layer.BorderColor = UIColor.LightGray.CGColor;
                //LogoImageView.Layer.BorderWidth = 1;
                //LogoImageView.Layer.CornerRadius = 5;

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
                    _descriptionTapGesture = new UITapGestureRecognizer(() => {
                        if (ViewModel.ExpandCollapseCommand.CanExecute(null))
                        {
                            ViewModel.ExpandCollapseCommand.Execute(null);
                        }
                    })
                    {
                        Enabled = IsDescriptionTapGestureEnabled
                    };

                    _descriptionTapGesture.NumberOfTouchesRequired = (uint)1;
                    _descriptionTapGesture.NumberOfTapsRequired = (uint)1;

                    DescriptionLabel.AddGestureRecognizer(_descriptionTapGesture);
                }

                return true;
            }

            return false;
        }

        private bool InitializeGoToContactButton()
        {
            if (GoToContactButton != null)
            {
                GoToContactButton.Layer.CornerRadius = 3;
                GoToContactButton.Layer.BorderWidth = 0;
                GoToContactButton.Layer.BackgroundColor = UIColor.FromRGB(230, 230, 230).CGColor;
                //GoToContactButton.colo

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
                SetCollectionViewCellWidth();

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

        private void SetCollectionViewCellWidth()
        {
            var flowLayout = (UICollectionViewFlowLayout)ContactCollectionView.CollectionViewLayout;
            var itemsInRow = ScreenUtil.IsVerticalOrientation ? 1 : 2;

            var cellWith = (ScreenUtil.CurrentScreenWidth - 
                            CollectionViewLeftConstraint.Constant -
                            Math.Abs(CollectionViewRightConstraint.Constant) -
                            flowLayout.SectionInset.Left -
                            flowLayout.SectionInset.Right - 
                            flowLayout.MinimumInteritemSpacing * (itemsInRow - 1)) / itemsInRow;

            flowLayout.ItemSize = new SizeF(cellWith, 41);
        }
    }
}