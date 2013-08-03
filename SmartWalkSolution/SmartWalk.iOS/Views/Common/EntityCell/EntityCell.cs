using System;
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

namespace SmartWalk.iOS.Views.Common.EntityCell
{
    public partial class EntityCell : TableCellBase
    {
        public const int DefaultLogoHeight = 240;

        private const int TextLineHeight = 19;
        private const int DefaultPagerHeight = 27;
        private const int Gap = 8;

        private const int MaxCollapsedCellHeight = 
            DefaultLogoHeight + 
            DefaultPagerHeight + 
            TextLineHeight * 3 + 
            Gap;

        public static readonly UINib Nib = UINib.FromName("EntityCell", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("EntityCell");

        private readonly MvxImageViewLoader _imageHelper;
        private readonly MKMapView _mapView;
        private readonly UIImageView _imageView;
        private readonly UICollectionView _collectionView;

        private UITapGestureRecognizer _descriptionTapGesture;
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
                        UpdateScrollViewHeightState(true);
                    }
                });

            _mapView = new MKMapView {
                ShowsUserLocation = true,
                UserInteractionEnabled = false
            };

            _imageView = new UIImageView {
                ContentMode = UIViewContentMode.ScaleAspectFit
            };

            var layout = new UICollectionViewFlowLayout {
                SectionInset = new UIEdgeInsets(8, 8, 8, 8),
                MinimumLineSpacing = 8,
                MinimumInteritemSpacing = 16
            };

            _collectionView = new UICollectionView(RectangleF.Empty, layout) {
                BackgroundColor = UIColor.White
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
                logoHeight = DefaultLogoHeight;
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
                var textSize = new NSString(text).StringSize(
                    UIFont.FromName("Helvetica", 15),
                    frameSize,
                    UILineBreakMode.TailTruncation);

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
            // TODO: Temporary disabled all contacts
            return false; //info != null && info.Contact != null && !info.Contact.IsEmpty;
        }

        private static bool HasAddress(EntityInfo info)
        {
            return info != null && info.Addresses != null && info.Addresses.Length > 0;
        }

        public Action<int, bool> ImageHeightUpdatedHandler { get; set; }
        public bool IsLogoSizeFixed { get; set; }
        public ICommand ExpandCollapseCommand { get; set; }

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

            _imageView.Image = null;
            _proportionalImageHeight = 0;
            ImageHeightUpdatedHandler = null;
            _mapView.RemoveAnnotations(_mapView.Annotations);
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            UpdateScrollViewHeightState();
            UpdateCollectionViewCellWidth();
            UpdateBottomGradientHiddenState();
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

            _imageHelper.ImageUrl = DataContext != null 
                ? DataContext.Entity.Info.Logo : null;

            DescriptionLabel.Text = DataContext != null 
                ? DataContext.Entity.Description : null;

            if (DataContext != null && 
                HasAddress(DataContextEntityInfo))
            {
                var annotation = new EntityAnnotation(
                    DataContext.Entity, 
                    DataContext.Entity.Info.Addresses[0]);
                _mapView.SetRegion(
                    MapUtil.CoordinateRegionForCoordinates(annotation.Coordinate), false);
                _mapView.AddAnnotation(annotation);
                _mapView.SelectAnnotation(annotation, false);
            }

            /*((ContactCollectionSource)_collectionView.WeakDataSource).ItemsSource =
                DataContext != null 
                    ? (IEnumerable)new ContactCollectionSourceConverter()
                    .Convert(DataContext.Entity.Info.Contact, typeof(IEnumerable), null, null) 
                    : null;*/

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
            if (DescriptionLabel.GestureRecognizers == null ||
                DescriptionLabel.GestureRecognizers.Length == 0)
            {
                _descriptionTapGesture = new UITapGestureRecognizer(() => {
                    if (ExpandCollapseCommand != null &&
                        ExpandCollapseCommand.CanExecute(null))
                    {
                        ExpandCollapseCommand.Execute(null);
                    }
                });

                _descriptionTapGesture.NumberOfTouchesRequired = (uint)1;
                _descriptionTapGesture.NumberOfTapsRequired = (uint)1;

                DescriptionLabel.AddGestureRecognizer(_descriptionTapGesture);
            }
        }

        private void InitializeContactCollectionView()
        {
            UpdateCollectionViewCellWidth();

            if (_collectionView.Source == null)
            {
                var collectionSource = new ContactCollectionSource(_collectionView);

                _collectionView.Source = collectionSource;
                _collectionView.Delegate = new ContactCollectionDelegate();

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

        private void UpdateCollectionViewCellWidth()
        {
            var flowLayout = (UICollectionViewFlowLayout)_collectionView.CollectionViewLayout;
            var itemsInRow = ScreenUtil.IsVerticalOrientation ? 1 : 2;

            var cellWith = (Frame.Width - 
                flowLayout.SectionInset.Left -
                flowLayout.SectionInset.Right - 
                flowLayout.MinimumInteritemSpacing * (itemsInRow - 1)) / itemsInRow;

            flowLayout.ItemSize = new SizeF(cellWith, 41);
        }

        private void UpdateScrollViewHeightState(bool updateTable = false)
        {
            _proportionalImageHeight = DefaultLogoHeight;
            if (!IsLogoSizeFixed && _imageView.Image != null)
            {
                var frame = _imageView.Layer.Frame;
                var imageSize = _imageView.SizeThatFits(frame.Size);
                _proportionalImageHeight = Math.Min(DefaultLogoHeight, 
                    (int)(1.0 * Frame.Width * imageSize.Height / imageSize.Width) + 1);
            }

            var isScrollVisible = IsScrollViewVisible(DataContextEntityInfo);
            var pagerHeight = CalculatePagerHeight(DataContextEntityInfo);

            ScrollViewHeightConstraint.Constant = 
                isScrollVisible ? _proportionalImageHeight + pagerHeight : 0;
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

            // TODO: It doesn't work on rotation
            _bottomGradient.Frame = BottomGradientView.Bounds;

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
    }
}