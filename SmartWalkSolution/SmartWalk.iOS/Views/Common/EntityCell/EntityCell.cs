using System;
using System.Drawing;
using System.Windows.Input;
using MonoTouch.CoreAnimation;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Core.Model;
using SmartWalk.Core.Utils;
using SmartWalk.iOS.Utils;

namespace SmartWalk.iOS.Views.Common.EntityCell
{
    public partial class EntityCell : TableCellBase
    {
        private const int ImageVerticalHeight = 120;
        private const int MapVerticalHeight = 80;
        private const int CellHorizontalHeight = 100;
        private const int TextLineHeight = 19;
        private const int Gap = 8;

        public static readonly UINib Nib = UINib.FromName("EntityCell", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("EntityCell");

        private UITapGestureRecognizer _descriptionTapGesture;
        private CAGradientLayer _bottomGradient;

        public EntityCell(IntPtr handle) : base(handle)
        {
        }

        public static EntityCell Create()
        {
            return (EntityCell)Nib.Instantiate(null, null)[0];
        }

        public static float CalculateCellHeight(
            float frameWidth,
            bool isExpanded, 
            Entity entity)
        {
            var textHeight = CalculateTextHeight(frameWidth - Gap * 2, entity.Description);
            var result = 
                GetHeaderHeight(entity) + 
                ((int)textHeight != 0 ? Gap * 2 : 0) + 
                (isExpanded ? textHeight : Math.Min(textHeight, TextLineHeight * 3));
            return result;
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

        private static float GetHeaderHeight(Entity entity)
        {
            if (entity.Info.HasLogo() || entity.Info.HasAddress())
            {
                return ScreenUtil.IsVerticalOrientation 
                    ? ImageVerticalHeight + MapVerticalHeight 
                    : CellHorizontalHeight;
            }

            return 0;
        }

        public ICommand ExpandCollapseCommand { get; set; }
        public ICommand ShowImageFullscreenCommand { get; set; }
        public ICommand ShowContactsViewCommand { get; set; }
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

        private ImageCell ImageCell
        {
            get { return (ImageCell)ImageCellPlaceholder.Content; }
        }

        private MapCell MapCell
        {
            get { return (MapCell)MapCellPlaceholder.Content; }
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            UpdateBottomGradientHiddenState();
            UpdateConstraints();
        }

        public override void UpdateConstraints()
        {
            base.UpdateConstraints();

            var headerHeight = DataContext != null
                ? GetHeaderHeight(DataContext.Entity)
                : 0;
            if (Frame.Height >= headerHeight)
            {
                HeaderHeightConstraint.Constant = headerHeight;
            }
            else
            {
                HeaderHeightConstraint.Constant = 0;
            }

            if (DataContext != null &&
                Frame.Height >= headerHeight)
            {
                if (DataContext.Entity.Info.HasLogo())
                {
                    if (ScreenUtil.IsVerticalOrientation)
                    {
                        ImageWidthConstraint.Constant = Bounds.Width;
                        ImageHeightConstraint.Constant = DataContext.Entity.Info.HasAddress()
                            ? ImageVerticalHeight
                            : ImageVerticalHeight + MapVerticalHeight;
                    }
                    else
                    {
                        ImageWidthConstraint.Constant = DataContext.Entity.Info.HasAddress()
                            ? Bounds.Width / 2
                            : Bounds.Width;
                        ImageHeightConstraint.Constant = CellHorizontalHeight;
                    }
                }
                else
                {
                    ImageWidthConstraint.Constant = 0;
                    ImageHeightConstraint.Constant = 0;
                }

                if (DataContext.Entity.Info.HasAddress())
                {
                    if (ScreenUtil.IsVerticalOrientation)
                    {
                        MapXConstraint.Constant = 0;
                        MapYConstraint.Constant = DataContext.Entity.Info.HasLogo()
                            ? ImageVerticalHeight
                            : 0;

                        MapWidthConstraint.Constant = Bounds.Width;
                        MapHeightConstraint.Constant = DataContext.Entity.Info.HasLogo()
                            ? MapVerticalHeight
                            : ImageVerticalHeight + MapVerticalHeight;
                    }
                    else
                    {
                        MapXConstraint.Constant = DataContext.Entity.Info.HasLogo()
                            ? Bounds.Width / 2
                            : 0;
                        MapYConstraint.Constant = 0;

                        MapWidthConstraint.Constant = DataContext.Entity.Info.HasLogo()
                            ? Bounds.Width / 2
                            : Bounds.Width;
                        MapHeightConstraint.Constant = CellHorizontalHeight;
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

            if (DataContext != null &&
                DataContext.Entity.Description != null)
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

        public override void WillMoveToSuperview(UIView newsuper)
        {
            base.WillMoveToSuperview(newsuper);

            if (newsuper == null)
            {
                ExpandCollapseCommand = null;
                ShowImageFullscreenCommand = null;
                ShowContactsViewCommand = null;
                NavigateWebSiteCommand = null;
                NavigateAddressesCommand = null;

                DisposeGestures();
                DisposeHeaderCells();
            }
        }

        protected override void OnInitialize()
        {
            InitializeGestures();
            InitializeHeaderCells();
            InitializeBottomGradientState();

            SetNeedsLayout();
        }

        protected override void OnDataContextChanged(object previousContext, object newContext)
        {
            DescriptionLabel.Text = DataContext != null 
                ? DataContext.Entity.Description : null;

            ImageCell.DataContext = DataContext != null &&
                    DataContext.Entity.Info.HasLogo()
                ? DataContext.Entity.Info
                : null;

            MapCell.DataContext = DataContext != null &&
                    DataContext.Entity.Info.HasAddress()
                ? DataContext.Entity
                : null;

            SetNeedsLayout();
            SetNeedsUpdateConstraints();
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
        }

        private void DisposeGestures()
        {
            if (_descriptionTapGesture != null)
            {
                DescriptionLabel.RemoveGestureRecognizer(_descriptionTapGesture);
                _descriptionTapGesture.Dispose();
                _descriptionTapGesture = null;
            }
        }

        private void InitializeHeaderCells()
        {
            ImageCellPlaceholder.Content = ImageCell.Create();
            MapCellPlaceholder.Content = MapCell.Create();

            ImageCell.ShowImageFullscreenCommand = ShowImageFullscreenCommand;
            ImageCell.ShowContactsViewCommand = ShowContactsViewCommand;
            ImageCell.NavigateWebSiteCommand = NavigateWebSiteCommand;
            MapCell.NavigateAddressesCommand = NavigateAddressesCommand;
        }

        private void DisposeHeaderCells()
        {
            ImageCell.ShowImageFullscreenCommand = null;
            ImageCell.NavigateWebSiteCommand = null;
            MapCell.NavigateAddressesCommand = null;

            ImageCell.Dispose();
            MapCell.Dispose();
            ImageCellPlaceholder.Content = null;
            MapCellPlaceholder.Content = null;
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
                    DataContext.Entity) - 
                        GetHeaderHeight(DataContext.Entity);

                _bottomGradient.Hidden = textHeight <= labelHeight;
            }
            else
            {
                _bottomGradient.Hidden = true;
            }
        }
    }
}