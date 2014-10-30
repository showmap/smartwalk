using System;
using System.Drawing;
using System.Windows.Input;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Client.Core.Model.DataContracts;
using SmartWalk.Client.Core.Utils;
using SmartWalk.Client.iOS.Resources;
using SmartWalk.Client.iOS.Utils;
using SmartWalk.Client.iOS.Views.Common.Base.Cells;

namespace SmartWalk.Client.iOS.Views.Common.EntityCell
{
    public partial class EntityCell : TableCellBase
    {
        private const float DefaultImageVerticalWidth = 320f;
        private const float DefaultImageVerticalHeight = 150f;
        private const int Gap = 10;

        public static readonly UINib Nib = UINib.FromName("EntityCell", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("EntityCell");

        // HACK: Can't call to Superview due to memory leak
        private WeakReference<UIView> _parentTableRef;
        private UITapGestureRecognizer _headerImageTapGesture;
        private UITapGestureRecognizer _descriptionTapGesture;

        public EntityCell(IntPtr handle) : base(handle)
        {
            BackgroundView = new UIView { BackgroundColor = UIColor.White };
        }

        public static EntityCell Create()
        {
            return (EntityCell)Nib.Instantiate(null, null)[0];
        }

        public static float CalculateCellHeight(
            SizeF containerSize,
            IEntityCellContext context)
        {
            var textHeight = CalculateTextHeight(containerSize.Width - Gap * 2, context.FullDescription());
            var result = 
                GetHeaderHeight(containerSize, context.Entity) + 
                ((int)textHeight != 0 ? Gap * 2 : 0) + 
                (context.IsDescriptionExpanded ?
                    textHeight 
                    : Math.Min(textHeight, Theme.EntityDescriptionFont.LineHeight * 3));
            return (float)Math.Ceiling(result);
        }

        private static float CalculateTextHeight(float frameWidth, string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                var frameSize = new SizeF(
                    frameWidth,
                    float.MaxValue); 

                SizeF textSize;

                using (var ns = new NSString(text))
                {
                    textSize = ns.StringSize(
                        Theme.EntityDescriptionFont,
                        frameSize,
                        UILineBreakMode.TailTruncation);
                }

                return textSize.Height;
            }

            return 0;
        }

        private static float GetHeaderHeight(SizeF containerSize, Entity entity)
        {
            if (ScreenUtil.IsVerticalOrientation)
            {
                var result = 
                    GetImageVerticalHeight(containerSize) +
                    (entity.HasAddresses() 
                        ? MapCell.DefaultHeight - 
                            (!entity.HasAddressText() ? MapCell.DefaultAddressHeight : 0)
                        : 0);
                return result;
            }

            var goldenHeight = ScreenUtil.GetGoldenRatio(containerSize.Height);
            return goldenHeight;
        }

        private static float GetImageVerticalHeight(SizeF containerSize)
        {
            var result = ScreenUtil.GetProportionalHeight(
                new SizeF(DefaultImageVerticalWidth, DefaultImageVerticalHeight), containerSize);
            return result;
        }

        public ICommand ExpandCollapseCommand { get; set; }
        public ICommand ShowImageFullscreenCommand { get; set; }
        public ICommand NavigateWebSiteCommand { get; set; }
        public ICommand NavigateAddressesCommand { get; set; }

        public new IEntityCellContext DataContext
        {
            get { return (IEntityCellContext)base.DataContext; }
            set { base.DataContext = value; }
        }

        private Entity DataContextEntity
        {
            get { return DataContext != null ? DataContext.Entity : null; }
        }

        private ImageBackgroundView ImageBackground
        {
            get { return (ImageBackgroundView)ImageCellPlaceholder.Content; }
        }

        private MapCell MapCell
        {
            get { return (MapCell)MapCellPlaceholder.Content; }
        }

        public override void WillMoveToSuperview(UIView newsuper)
        {
            base.WillMoveToSuperview(newsuper);

            _parentTableRef = newsuper != null
                ? new WeakReference<UIView>(newsuper)
                : null;

            if (newsuper == null)
            {
                ExpandCollapseCommand = null;
                ShowImageFullscreenCommand = null;
                NavigateWebSiteCommand = null;
                NavigateAddressesCommand = null;

                DisposeGestures();
                DisposeHeaderImage();
                DisposeMapCell();
            }
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            UpdateConstraints();
        }

        public override void UpdateConstraints()
        {
            base.UpdateConstraints();

            UIView parentTable;
            if (_parentTableRef == null || !_parentTableRef.TryGetTarget(out parentTable)) return;

            var entity = DataContext != null ? DataContext.Entity : null;
            var headerHeight = GetHeaderHeight(parentTable.Frame.Size, entity);
            if (Frame.Height >= headerHeight)
            {
                HeaderHeightConstraint.Constant = headerHeight;
            }
            else
            {
                HeaderHeightConstraint.Constant = 0;
            }

            var imageVerticalHeight = GetImageVerticalHeight(parentTable.Frame.Size);
            if (entity != null && Frame.Height >= headerHeight)
            {
                if (ScreenUtil.IsVerticalOrientation)
                {
                    ImageWidthConstraint.Constant = Bounds.Width;
                    ImageHeightConstraint.Constant = imageVerticalHeight;
                }
                else
                {
                    ImageWidthConstraint.Constant = entity.HasAddresses()
                        ? Bounds.Width / 2
                        : Bounds.Width;
                    ImageHeightConstraint.Constant = headerHeight;
                }

                if (entity.HasAddresses())
                {
                    if (ScreenUtil.IsVerticalOrientation)
                    {
                        MapXConstraint.Constant = 0;
                        MapYConstraint.Constant = imageVerticalHeight;

                        MapWidthConstraint.Constant = Bounds.Width;
                        MapHeightConstraint.Constant = MapCell.DefaultHeight
                            - (!entity.HasAddressText() ? MapCell.DefaultAddressHeight : 0);
                    }
                    else
                    {
                        MapXConstraint.Constant = Bounds.Width / 2;
                        MapYConstraint.Constant = 0;

                        MapWidthConstraint.Constant = Bounds.Width / 2;
                        MapHeightConstraint.Constant = headerHeight;
                    }
                }
                else
                {
                    MapWidthConstraint.Constant = 0;
                    MapHeightConstraint.Constant = 0;
                }
            }
            else
            {
                ImageWidthConstraint.Constant = 0;
                ImageHeightConstraint.Constant = 0;
                MapXConstraint.Constant = 0;
                MapYConstraint.Constant = 0;
                MapWidthConstraint.Constant = 0;
                MapHeightConstraint.Constant = 0;
            }

            // HACK: to make sure that Frames are updated (on first opening they aren't)
            ImageCellPlaceholder.Frame = new RectangleF(
                0, 
                0, 
                ImageWidthConstraint.Constant,
                ImageHeightConstraint.Constant);
            MapCellPlaceholder.Frame = new RectangleF(
                MapXConstraint.Constant,
                MapYConstraint.Constant,
                MapWidthConstraint.Constant,
                MapHeightConstraint.Constant);

            if (entity != null && entity.Description != null)
            {
                DescriptionTopConstraint.Constant = Gap;
                DescriptionBottomConstraint.Constant = Gap;
            }
            else
            {
                DescriptionTopConstraint.Constant = 0;
                DescriptionBottomConstraint.Constant = 0;
            }
        }

        protected override void OnInitialize()
        {
            InitializeStyle();
            InitializeHeaderImage();
            InitializeMapCell();
            InitializeGestures();

            SetNeedsLayout();
        }

        protected override void OnDataContextChanged(object previousContext, object newContext)
        {
            DescriptionLabel.Text = DataContext != null 
                ? DataContext.FullDescription()
                : null;

            ImageBackground.Title = DataContext != null
                ? DataContext.Title
                : null;

            ImageBackground.Subtitle = DataContext != null
                ? DataContext.Subtitle
                : null;

            ImageBackground.ImageUrl = DataContext != null
                ? DataContext.Entity.Picture
                : null;

            MapCell.DataContext = DataContext != null && DataContext.Entity.HasAddresses()
                ? DataContext.Entity
                : null;

            SetNeedsLayout();
            SetNeedsUpdateConstraints();
        }

        private void InitializeGestures()
        {
            _headerImageTapGesture = new UITapGestureRecognizer(() => {
                if (ShowImageFullscreenCommand != null &&
                    ShowImageFullscreenCommand.CanExecute(ImageBackground.ImageUrl))
                {
                    ShowImageFullscreenCommand.Execute(ImageBackground.ImageUrl);
                }
            }) {
                NumberOfTouchesRequired = (uint)1,
                NumberOfTapsRequired = (uint)1
            };

            ImageBackground.AddGestureRecognizer(_headerImageTapGesture);

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
        }

        private void DisposeGestures()
        {
            if (_headerImageTapGesture != null)
            {
                ImageBackground.RemoveGestureRecognizer(_headerImageTapGesture);
                _headerImageTapGesture.Dispose();
                _headerImageTapGesture = null;
            }

            if (_descriptionTapGesture != null)
            {
                DescriptionLabel.RemoveGestureRecognizer(_descriptionTapGesture);
                _descriptionTapGesture.Dispose();
                _descriptionTapGesture = null;
            }
        }

        private void InitializeHeaderImage()
        {
            ImageCellPlaceholder.Content = ImageBackgroundView.Create();
            ImageBackground.Initialize();
        }

        private void DisposeHeaderImage()
        {
            ImageBackground.Dispose();
            ImageCellPlaceholder.Content = null;
        }

        private void InitializeMapCell()
        {
            MapCellPlaceholder.Content = MapCell.Create();

            MapCell.NavigateAddressesCommand = NavigateAddressesCommand;
        }

        private void DisposeMapCell()
        {
            MapCell.NavigateAddressesCommand = null;

            MapCell.Dispose();
            MapCellPlaceholder.Content = null;
        }

        private void InitializeStyle()
        {
            DescriptionLabel.Font = Theme.EntityDescriptionFont;
            DescriptionLabel.TextColor = Theme.EntityDescription;
        }
    }
}