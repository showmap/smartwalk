using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Input;
using Cirrious.MvvmCross.Binding.Touch.Views;
using MonoTouch.CoreAnimation;
using MonoTouch.Foundation;
using MonoTouch.MapKit;
using MonoTouch.UIKit;
using SmartWalk.Core.Model;
using SmartWalk.iOS.Utils;
using SmartWalk.iOS.Controls;
using SmartWalk.Core.ViewModels;
using SmartWalk.Core.Model.Interfaces;

namespace SmartWalk.iOS.Views.Common.EntityCell
{
    public partial class EntityCell : TableCellBase
    {
        public const int DefaultImageHeight = 240;

        private const int TextLineHeight = 19;
        private const int DefaultPagerHeight = 27;
        private const int Gap = 8;

        private const int MaxCollapsedCellHeight = 
            DefaultImageHeight + 
            DefaultPagerHeight + 
            TextLineHeight * 3 + 
            Gap;

        public static readonly UINib Nib = UINib.FromName("EntityCell", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("EntityCell");

        private readonly MvxImageViewLoader _imageHelper;
        private MKMapView _mapView;
        private ProgressImageView _imageView;
        private ExtendedCollectionView _collectionView;

        private UITapGestureRecognizer _descriptionTapGesture;
        private UITapGestureRecognizer _imageTapGesture;
        private UITapGestureRecognizer _mapTapGesture;
        private CAGradientLayer _bottomGradient;
        private int _proportionalImageHeight;

        public EntityCell(IntPtr handle) : base(handle)
        {
            _imageHelper = new MvxImageViewLoader(
                () => _imageView,
                () => {
                    if (_imageHelper.ImageUrl != null && 
                        _imageView.Image != null)
                    {
                        _imageView.StopProgress();
                        UpdateScrollViewHeightState(true);
                    }
                    else if (_imageHelper.ImageUrl == null)
                    {
                        _imageView.StopProgress();
                    }
                    else
                    {
                        _imageView.StartProgress();
                    }
                });

            _mapView = new MKMapView {
                ShowsUserLocation = true,
                UserInteractionEnabled = true,
                ZoomEnabled = false,
                ScrollEnabled = false
            };

            _imageView = new ProgressImageView {
                ContentMode = UIViewContentMode.ScaleAspectFit,
                UserInteractionEnabled = true
            };

            var layout = new UICollectionViewFlowLayout {
                SectionInset = new UIEdgeInsets(8, 8, 8, 8),
                MinimumLineSpacing = 8,
                MinimumInteritemSpacing = 16
            };

            _collectionView = new ExtendedCollectionView(RectangleF.Empty, layout) {
                BackgroundColor = UIColor.White,
                CellHeight = 41
            };
        }

        public static EntityCell Create()
        {
            return (EntityCell)Nib.Instantiate(null, null)[0];
        }

        public static float CalculateCellHeight(
            float frameWidth,
            bool isExpanded, 
            Entity entity, 
            int logoHeight)
        {
            if (logoHeight == 0)
            {
                logoHeight = DefaultImageHeight;
            }

            var noTextCellHeight = CalculateNoTextCellHeight(entity.Info, logoHeight);
            var textHeight = CalculateTextHeight(frameWidth - Gap * 2, entity.Description);
            var linesCount = (int)Math.Round(
                1.0 * (MaxCollapsedCellHeight - noTextCellHeight) / TextLineHeight, 
                MidpointRounding.AwayFromZero);

            return isExpanded 
                ? noTextCellHeight + textHeight
                    : noTextCellHeight + Math.Min(textHeight, TextLineHeight * linesCount);
        }

        private static int CalculatePagesCount(EntityInfo info)
        {
            return info != null ? 
                (HasAddress(info) ? 1 : 0) +
                    (HasLogo(info) ? 1 : 0) +
                    (HasContact(info) ? 1 : 0) : 0;
        }

        private static int CalculatePagerHeight(EntityInfo info)
        {
            return CalculatePagesCount(info) < 2 ? 0 : DefaultPagerHeight;
        }

        private static float CalculateNoTextCellHeight(EntityInfo info, int logoHeight)
        {
            var isScrollVisible = IsScrollViewVisible(info);
            var pagerHeight = CalculatePagerHeight(info);
            pagerHeight = pagerHeight == 0 ? Gap : pagerHeight;
            var noTextCellHeight = (isScrollVisible ? logoHeight + pagerHeight : Gap) + Gap;
            return noTextCellHeight;
        }

        private static float CalculateTextHeight(float frameWidth, string text)
        {
            if (text != null && text != string.Empty)
            {
                var frameSize = new SizeF(
                    frameWidth,
                    float.MaxValue); 

                SizeF textSize;
                using (var ns = new NSString(text))
                {
                    textSize = ns.StringSize(
                        UIFont.FromName("Helvetica", 15),
                        frameSize,
                        UILineBreakMode.TailTruncation);
                }

                return textSize.Height;
            }

            return 0;
        }

        private static bool IsScrollViewVisible(EntityInfo info)
        {
            return info != null && (HasLogo(info) || HasContact(info) || HasAddress(info));
        }

        private static bool HasLogo(EntityInfo info)
        {
            return info != null && info.Logo != null;
        }

        private static bool HasContact(EntityInfo info)
        {
            return info != null && info.Contact != null && !info.Contact.IsEmpty;
        }

        private static bool HasAddress(EntityInfo info)
        {
            return info != null && info.Addresses != null && info.Addresses.Length > 0;
        }

        public Action<int, bool> ImageHeightUpdatedHandler { get; set; }
        public bool IsLogoSizeFixed { get; set; }
        public ICommand ExpandCollapseCommand { get; set; }
        public ICommand ShowImageFullscreenCommand { get; set; }
        public ICommand NavigateWebSiteCommand { get; set; }
        public ICommand NavigateAddressesCommand { get; set; }

        public new IEntityCellContext DataContext
        {
            get { return (IEntityCellContext)base.DataContext; }
            set { base.DataContext = value; }
        }

        private EntityInfo DataContextEntityInfo
        {
            get { return DataContext != null ? DataContext.Entity.Info : null; }
        }

        public override void PrepareForReuse()
        {
            base.PrepareForReuse();

            _proportionalImageHeight = DefaultImageHeight;
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            UpdateScrollViewHeightState();
            UpdateBottomGradientHiddenState();
        }

        public override void WillMoveToSuperview(UIView newsuper)
        {
            base.WillMoveToSuperview(newsuper);

            if (newsuper == null)
            {
                ImageHeightUpdatedHandler = null;
                ExpandCollapseCommand = null;
                ShowImageFullscreenCommand = null;
                NavigateWebSiteCommand = null;
                NavigateAddressesCommand = null;

                DisposeGestures();
            }
        }

        protected override void OnInitialize()
        {
            InitializeGestures();
            InitializeContactCollectionView();
            InitializeBottomGradientState();

            SetNeedsLayout();
        }

        protected override void OnDataContextChanged(object previousContext, object newContext)
        {
            PopulateScrollView();

            _imageView.Image = null;
            _mapView.RemoveAnnotations(_mapView.Annotations);

            _imageHelper.ImageUrl = DataContext != null 
                ? DataContext.Entity.Info.Logo : null;

            DescriptionLabel.Text = DataContext != null 
                ? DataContext.Entity.Description : null;

            if (DataContext != null && 
                HasAddress(DataContextEntityInfo))
            {
                _mapView.RemoveAnnotations(_mapView.Annotations);
                var annotation = new EntityAnnotation(
                    DataContext.Entity, 
                    DataContext.Entity.Info.Addresses[0]);
                _mapView.SetRegion(
                    MapUtil.CoordinateRegionForCoordinates(annotation.Coordinate), false);
                _mapView.AddAnnotation(annotation);
                _mapView.SelectAnnotation(annotation, false);
            }

            ((ContactCollectionSource)_collectionView.WeakDataSource).ItemsSource =
                DataContext != null 
                    ? (IEnumerable)new ContactCollectionSourceConverter()
                    .Convert(DataContext.Entity.Info.Contact, typeof(IEnumerable), null, null) 
                    : null;

            ScrollView.CurrentPage = 
                HasAddress(DataContextEntityInfo) && 
                    HasLogo(DataContextEntityInfo) 
                    ? 1 : 0;

            SetNeedsLayout();
        }

        private void PopulateScrollView()
        {
            var pages = new List<UIView>();

            if (HasAddress(DataContextEntityInfo))
            {
                pages.Add(_mapView);
            }

            if (HasLogo(DataContextEntityInfo))
            {
                pages.Add(_imageView);
            }

            if (HasContact(DataContextEntityInfo))
            {
                pages.Add(_collectionView);
            }

            ScrollView.PageViews = pages.Count > 0 ? pages.ToArray() : null;
        }

        private void InitializeGestures()
        {
            _descriptionTapGesture = new UITapGestureRecognizer(() => {
                if (ExpandCollapseCommand != null &&
                    ExpandCollapseCommand.CanExecute(null))
                {
                    ExpandCollapseCommand.Execute(null);
                }
            }) {
                NumberOfTouchesRequired = (uint)1,
                NumberOfTapsRequired = (uint)1
            };

            DescriptionLabel.AddGestureRecognizer(_descriptionTapGesture);

            _imageTapGesture = new UITapGestureRecognizer(() => {
                if (ShowImageFullscreenCommand.CanExecute(DataContext.Entity.Info.Logo))
                {
                    ShowImageFullscreenCommand.Execute(DataContext.Entity.Info.Logo);
                }
            }) {
                NumberOfTouchesRequired = (uint)1,
                NumberOfTapsRequired = (uint)1
            };

            _imageView.AddGestureRecognizer(_imageTapGesture);

            _mapTapGesture = new UITapGestureRecognizer(() => {
                if (NavigateAddressesCommand.CanExecute(DataContext.Entity))
                {
                    NavigateAddressesCommand.Execute(DataContext.Entity);
                }
            }) {
                NumberOfTouchesRequired = (uint)1,
                NumberOfTapsRequired = (uint)1
            };

            _mapView.AddGestureRecognizer(_mapTapGesture);
        }

        private void DisposeGestures()
        {
            if (_descriptionTapGesture != null)
            {
                DescriptionLabel.RemoveGestureRecognizer(_descriptionTapGesture);
                _descriptionTapGesture.Dispose();
                _descriptionTapGesture = null;
            }

            if (_imageTapGesture != null)
            {
                _imageView.RemoveGestureRecognizer(_imageTapGesture);
                _imageTapGesture.Dispose();
                _imageTapGesture = null;
            }

            if (_mapTapGesture != null)
            {
                _mapView.RemoveGestureRecognizer(_mapTapGesture);
                _mapTapGesture.Dispose();
                _mapTapGesture = null;
            }
        }

        private void InitializeContactCollectionView()
        {
            if (_collectionView.Source == null)
            {
                var collectionSource = new ContactCollectionSource(_collectionView);

                _collectionView.Source = collectionSource;

                _collectionView.Delegate = new ContactCollectionDelegate(collectionSource) {
                    NavigateSiteLinkCommand = NavigateWebSiteCommand
                };

                _collectionView.ReloadData();
            }
        }

        private void InitializeBottomGradientState()
        {
            _bottomGradient = new CAGradientLayer
                {
                    Frame = BottomGradientView.Bounds,
                    Colors = new [] { 
                        UIColor.White.ColorWithAlpha(0.2f).CGColor, 
                        UIColor.White.CGColor 
                    },
                };

            BottomGradientView.Layer.InsertSublayer(_bottomGradient, 0);
        }

        private void UpdateScrollViewHeightState(bool updateTable = false)
        {
            _proportionalImageHeight = GetImageProportionalHeight();

            var isScrollVisible = IsScrollViewVisible(DataContextEntityInfo);
            var pagerHeight = CalculatePagerHeight(DataContextEntityInfo);

            ScrollViewHeightConstraint.Constant = 
                isScrollVisible ? _proportionalImageHeight + pagerHeight : 0;

            // HACK: Setting Frame implicitly to trigger re-layout on rotation
            ScrollView.Frame = new RectangleF(
                0,
                0,
                Frame.Width,
                ScrollViewHeightConstraint.Constant);

            DescriptionTopSpaceConstraint.Constant = 
                isScrollVisible && pagerHeight > 0 ? 0 : Gap;

            if (!IsLogoSizeFixed && 
                ImageHeightUpdatedHandler != null)
            {
                ImageHeightUpdatedHandler(_proportionalImageHeight, updateTable);
            }
        }

        private void UpdateBottomGradientHiddenState()
        {
            if (_bottomGradient == null) return;

            // HACK: getting width from Cell's bounds, because Label's ones aren't updated yet
            var frame = _bottomGradient.Frame;
            frame.Width = Bounds.Width - Gap * 2;
            _bottomGradient.Frame = frame;

            if (DataContext != null && 
                !DataContext.IsDescriptionExpanded)
            {
                var textHeight = CalculateTextHeight(
                    DescriptionLabel.Frame.Width, 
                    DataContext.Entity.Description);

                var labelHeight = CalculateCellHeight(
                    Frame.Width, 
                    false, 
                    DataContext.Entity, 
                    _proportionalImageHeight) - 
                        CalculateNoTextCellHeight(
                            DataContext.Entity.Info,
                            _proportionalImageHeight);

                _bottomGradient.Hidden = textHeight <= labelHeight;
            }
            else
            {
                _bottomGradient.Hidden = true;
            }
        }

        private int GetImageProportionalHeight()
        {
            if (_imageHelper.ImageUrl != null &&
                _imageView.Image != null &&
                !IsLogoSizeFixed)
            {
                var imageSize = _imageView.Image.Size;
                var height = Math.Min(DefaultImageHeight, 
                    (int)(1.0 * Frame.Width * imageSize.Height / imageSize.Width) + 1);
                return height;
            }

            return DefaultImageHeight;
        }
    }
}