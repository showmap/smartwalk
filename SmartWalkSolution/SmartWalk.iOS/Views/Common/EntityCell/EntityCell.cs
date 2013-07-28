using System;
using System.Collections;
using System.Drawing;
using Cirrious.CrossCore.Core;
using Cirrious.MvvmCross.Binding.Touch.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Core.Model;
using SmartWalk.Core.ViewModels;
using SmartWalk.iOS.Utils;

namespace SmartWalk.iOS.Views.Common.EntityCell
{
    public partial class EntityCell : TableCellBase<EntityViewModel>
    {
        public const int DefaultLogoHeight = 240;
        private const int MaxLogoHeight = 280;
        private const int PagerHeight = 27;
        private const int Gap = 8;

        public static readonly UINib Nib = UINib.FromName("EntityCell", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("EntityCell");

        private readonly MvxImageViewLoader _imageHelper;

        private UITapGestureRecognizer _descriptionTapGesture;

        public EntityCell(IntPtr handle) : base(handle)
        {
            _imageHelper = new MvxImageViewLoader(() => LogoImageView, UpdateScrollViewHeightState);
        }

        public event EventHandler<MvxValueEventArgs<int>> ImageHeightUpdated;

        public static EntityCell Create()
        {
            return (EntityCell)Nib.Instantiate(null, null)[0];
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

        public bool IsLogoSizeFixed { get; set; }

        public static float CalculateCellHeight(
            bool isExpanded, 
            Entity entity, 
            float logoHeight = DefaultLogoHeight)
        {
            var isScrollVisible = IsScrollViewVisible(entity.Info);
            var cellHeight = (isScrollVisible ? logoHeight + PagerHeight : Gap) + Gap;
            var textHeight = default(float);

            if (entity.Description != null && 
                entity.Description != string.Empty)
            {
                var frameSize = new SizeF(ScreenUtil.CurrentScreenWidth - Gap * 2, float.MaxValue); 
                var textSize = new NSString(entity.Description).StringSize(
                    UIFont.FromName("Helvetica", 15),
                    frameSize,
                    UILineBreakMode.TailTruncation);

                textHeight = textSize.Height;
            }

            var threeLinesHeight = 57.0f;

            return isExpanded 
                ? cellHeight + textHeight
                : cellHeight + Math.Min(textHeight, threeLinesHeight);
        }

        protected override void OnInitialize()
        {
            InitializeGestures();
            InitializeScrollView();
            InitializeContactCollectionView();
        }

        protected override void OnDataContextChanged()
        {
            _imageHelper.ImageUrl = DataContext != null 
                ? DataContext.Entity.Info.Logo : null;

            DescriptionLabel.Text = DataContext != null 
                ? DataContext.Entity.Description : null;

            _descriptionTapGesture.Enabled = DataContext != null 
                ? DataContext.IsDescriptionExpandable : false;

            PageControl.Hidden = DataContext != null && 
                    DataContext.Entity.Info != null 
                ? !IsScrollViewVisible(DataContext.Entity.Info) 
                : false;

            ((ContactCollectionSource)ContactCollectionView.WeakDataSource).ItemsSource =
                DataContext != null 
                    ? (IEnumerable)new ContactCollectionSourceConverter()
                    .Convert(DataContext.Entity.Info.Contact, typeof(IEnumerable), null, null) 
                    : null;

            UpdateScrollViewHeightState();
        }

        private static bool IsScrollViewVisible(EntityInfo info)
        {
            return info.Logo != null || (info.Contact != null && !info.Contact.IsEmpty);
        }

        private void InitializeGestures()
        {
            if (DescriptionLabel.GestureRecognizers == null ||
                DescriptionLabel.GestureRecognizers.Length == 0)
            {
                _descriptionTapGesture = new UITapGestureRecognizer(() => {
                    if (DataContext != null &&
                        DataContext.ExpandCollapseCommand.CanExecute(null))
                    {
                        DataContext.ExpandCollapseCommand.Execute(null);
                    }
                });

                _descriptionTapGesture.NumberOfTouchesRequired = (uint)1;
                _descriptionTapGesture.NumberOfTapsRequired = (uint)1;

                DescriptionLabel.AddGestureRecognizer(_descriptionTapGesture);
            }
        }

        private void InitializeScrollView()
        {
            if (ScrollView.Delegate == null)
            {
                ScrollView.Delegate = new EntityScrollViewDelegate(ScrollView, PageControl);
            }
        }

        private void InitializeContactCollectionView()
        {
            SetCollectionViewCellWidth();

            if (ContactCollectionView.Source == null)
            {
                var collectionSource = new ContactCollectionSource(ContactCollectionView);

                ContactCollectionView.Source = collectionSource;
                ContactCollectionView.Delegate = new ContactCollectionDelegate();

                ContactCollectionView.ReloadData();
            }
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
                flowLayout.SectionInset.Left -
                flowLayout.SectionInset.Right - 
                flowLayout.MinimumInteritemSpacing * (itemsInRow - 1)) / itemsInRow;

            flowLayout.ItemSize = new SizeF(cellWith, 41);
        }

        private void UpdateScrollViewHeightState()
        {
            var height = DefaultLogoHeight;
            if (!IsLogoSizeFixed)
            {
                if (LogoImageView.Image != null)
                {
                    var frame = LogoImageView.Layer.Frame;
                    var imageSize = LogoImageView.SizeThatFits(frame.Size);
                    height = Math.Min(MaxLogoHeight, 
                        (int)(1.0 * ScreenUtil.CurrentScreenWidth * imageSize.Height / imageSize.Width) + 1);
                }
            }

            var isScrollVisible = DataContext != null &&
                DataContext.Entity.Info != null && 
                IsScrollViewVisible(DataContext.Entity.Info);
            var newHeight = isScrollVisible ? height : 0;
            ScrollViewHeightConstraint.Constant = newHeight;
            DescriptionTopSpaceConstraint.Constant = isScrollVisible ? 27 : 8;

            if (!IsLogoSizeFixed && 
                newHeight != DefaultLogoHeight && 
                ImageHeightUpdated != null)
            {
                ImageHeightUpdated(this,
                    new MvxValueEventArgs<int>(newHeight));
            }
        }
    }
}