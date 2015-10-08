using System;
using CoreGraphics;
using System.Windows.Input;
using Foundation;
using UIKit;
using SmartWalk.Client.Core.Model.DataContracts;
using SmartWalk.Client.Core.Utils;
using SmartWalk.Shared.Utils;
using SmartWalk.Client.iOS.Resources;
using SmartWalk.Client.iOS.Utils;
using SmartWalk.Client.iOS.Views.Common.Base.Cells;
using SmartWalk.Client.Core.ViewModels.Interfaces;

namespace SmartWalk.Client.iOS.Views.Common.EntityCell
{
    public partial class EntityCell : TableCellBase
    {
        private const float DefaultImageVerticalWidth = 320f;
        private const float DefaultImageVerticalHeight = 150f;
        private const int Gap = 10;

        private static readonly float DefaultDescriptionTextHeight = 
            ScreenUtil.CalculateTextHeight(
                300, 
                string.Format("Ap{0}Bp{0}Cp", Environment.NewLine),
                Theme.EntityDescriptionFont); // 3 rows

        public static readonly UINib Nib = UINib.FromName("EntityCell", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("EntityCell");

        private UITapGestureRecognizer _headerImageTapGesture;
        private UITapGestureRecognizer _descriptionTapGesture;
        private CGSize _lastLayoutSize;

        private ICommand _navigateAddressesCommand;

        public EntityCell(IntPtr handle) : base(handle)
        {
            BackgroundView = new UIView { BackgroundColor = UIColor.White };
        }

        public static float CalculateCellHeight(CGSize frame, IEntityCellContext context)
        {
            var descriptionHeight = GetDescriptionHeight(frame.Width, context);
            var result = 
                GetHeaderHeight(frame, context.Entity) + 
                    descriptionHeight;
            return result;
        }

        private static float GetHeaderHeight(CGSize frame, Entity entity)
        {
            if (ScreenUtil.IsVerticalOrientation)
            {
                var result = 
                    GetImageVerticalHeight(frame.Width) +
                    (entity.HasAddresses() 
                        ? MapCell.DefaultHeight - 
                            (!entity.HasAddressText() ? MapCell.DefaultAddressHeight : 0)
                        : 0);
                return result;
            }

            var goldenHeight = ScreenUtil.GetGoldenRatio(frame.Height);
            return goldenHeight;
        }

        private static float GetDescriptionHeight(nfloat frameWidth, IEntityCellContext context)
        {
            var textHeight = ScreenUtil.CalculateTextHeight(
                frameWidth - Gap * 2, 
                context.FullDescription(), 
                Theme.EntityDescriptionFont);
            var result = (!textHeight.EqualsF(0) ? Gap * 2 : 0) + 
                (context.IsDescriptionExpanded ?
                    textHeight 
                    : Math.Min(textHeight, DefaultDescriptionTextHeight));
            return result;
        }

        private static float GetImageVerticalHeight(nfloat frameWidth)
        {
            var result = ScreenUtil.GetProportionalHeight(
                new CGSize(DefaultImageVerticalWidth, DefaultImageVerticalHeight), frameWidth);
            return (float)Math.Ceiling(result);
        }

        public ICommand ExpandCollapseCommand { get; set; }
        public ICommand ShowHideModalViewCommand { get; set; }

        public ICommand NavigateAddressesCommand
        {
            get { return _navigateAddressesCommand; }
            set 
            {
                _navigateAddressesCommand = value;
                InitializeMapCell();
            }
        }

        public new IEntityCellContext DataContext
        {
            get { return (IEntityCellContext)base.DataContext; }
            set { base.DataContext = value; }
        }

        private Entity DataContextEntity
        {
            get { return DataContext != null ? DataContext.Entity : null; }
        }

        public override void WillMoveToSuperview(UIView newsuper)
        {
            base.WillMoveToSuperview(newsuper);

            if (newsuper == null)
            {
                ExpandCollapseCommand = null;
                ShowHideModalViewCommand = null;
                NavigateAddressesCommand = null;

                DisposeGestures();
                DisposeMapCell();

                ImageBackground.Dispose();
            }
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            if (_lastLayoutSize != Bounds.Size)
            {
                UpdateConstraintConstants();
                _lastLayoutSize = Bounds.Size;
            }
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            InitializeStyle();
            InitializeMapCell();
            InitializeGestures();
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

            _lastLayoutSize = CGSize.Empty;
            SetNeedsLayout();
        }

        private void UpdateConstraintConstants()
        {
            var headerHeight = Bounds.Height - 
                (DataContext != null ? GetDescriptionHeight(Bounds.Width, DataContext) : 0);
            var imageVerticalHeight = GetImageVerticalHeight(Bounds.Width);

            HeaderHeightConstraint.Constant = headerHeight;

            var entity = DataContext != null ? DataContext.Entity : null;
            if (entity != null)
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

            if (DataContext != null && DataContext.FullDescription() != null)
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

        private void InitializeGestures()
        {
            _headerImageTapGesture = new UITapGestureRecognizer(() => {
                var context = new ModalViewContext(
                    ModalViewKind.FullscreenImage, 
                    ImageBackground.ImageUrl);
                
                if (ShowHideModalViewCommand != null &&
                    ShowHideModalViewCommand.CanExecute(context))
                {
                    ShowHideModalViewCommand.Execute(context);
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

        private void InitializeMapCell()
        {
            MapCell.NavigateAddressesCommand = NavigateAddressesCommand;
        }

        private void DisposeMapCell()
        {
            MapCell.NavigateAddressesCommand = null;
            MapCell.Dispose();
        }

        private void InitializeStyle()
        {
            DescriptionLabel.Font = Theme.EntityDescriptionFont;
            DescriptionLabel.TextColor = ThemeColors.ContentLightTextPassive;
        }
    }
}