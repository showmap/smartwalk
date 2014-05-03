using System;
using System.Drawing;
using System.Windows.Input;
using MonoTouch.CoreAnimation;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Client.Core.Model.DataContracts;
using SmartWalk.Client.iOS.Resources;
using SmartWalk.Client.iOS.Views.Common.Base;

namespace SmartWalk.Client.iOS.Views.Common.EntityCell
{
    public partial class EntityCell : TableCellBase
    {
        private const int ImageHeight = 200;
        private const int Gap = 10;

        public static readonly UINib Nib = UINib.FromName("EntityCell", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("EntityCell");

        private UITapGestureRecognizer _descriptionTapGesture;
        private CAGradientLayer _bottomGradient;

        public EntityCell(IntPtr handle) : base(handle)
        {
            BackgroundView = new UIView { BackgroundColor = Theme.BackgroundPatternColor };
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
                ImageHeight + 
                ((int)textHeight != 0 ? Gap * 2 : 0) + 
                    (isExpanded ? textHeight : Math.Min(textHeight, Theme.EntityDescrFont.LineHeight * 3));
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

                // TODO: iOS 7 text measuring is a broken shit
                /*if (UIDevice.CurrentDevice.CheckSystemVersion(7, 0))
                {
                    using (var ns = new NSMutableAttributedString(
                        text, 
                        new UIStringAttributes { 
                            Font = Theme.EntityDescrFont,
                            ParagraphStyle = new NSMutableParagraphStyle {
                                Alignment = UITextAlignment.Left,
                                LineBreakMode = UILineBreakMode.TailTruncation,
                            }
                        }))
                    {
                        textSize = ns.GetBoundingRect(
                            frameSize,
                            NSStringDrawingOptions.UsesLineFragmentOrigin |
                            NSStringDrawingOptions.UsesFontLeading,
                            null).Size;
                    }
                }
                else*/
                {
                    using (var ns = new NSString(text))
                    {
                        textSize = ns.StringSize(
                            Theme.EntityDescrFont,
                            frameSize,
                            UILineBreakMode.TailTruncation);
                    }
                }

                return textSize.Height;
            }

            return 0;
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
            get { return (ImageBackgroundView)ImagePlaceholder.Content; }
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
                NavigateWebSiteCommand = null;
                NavigateAddressesCommand = null;

                DisposeGestures();
                DisposeHeaderImage();
            }
        }

        protected override void OnInitialize()
        {
            InitializeStyle();
            InitializeGestures();
            InitializeHeaderImage();
            InitializeBottomGradientState();

            SetNeedsLayout();
        }

        protected override void OnDataContextChanged(object previousContext, object newContext)
        {
            DescriptionLabel.Text = DataContext != null 
                ? DataContext.Entity.Description : null;

            ImageBackground.Title = DataContext != null
                ? DataContext.Title
                : null;

            if (DataContext.Mode == EntityViewModelWrapper.ModelMode.Event)
            {
                ImageBackground.Uptitle = DataContext != null
                    ? DataContext.Subtitle
                    : null;
            }
            else
            {
                ImageBackground.Subtitle = DataContext != null
                    ? DataContext.Subtitle
                    : null;
            }

            ImageBackground.SubtitleButtonImage = 
                DataContext != null &&
                DataContext.Mode == EntityViewModelWrapper.ModelMode.Venue
                    ? ThemeIcons.NavBarMapLandscape
                    : null;

            ImageBackground.ImageUrl = DataContext != null
                ? DataContext.Entity.Picture
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

        private void InitializeHeaderImage()
        {
            ImagePlaceholder.Content = ImageBackgroundView.Create();

            ImageBackground.Initialize();
            ImageBackground.ShowImageFullscreenCommand = ShowImageFullscreenCommand;

            if (DataContext != null &&
                DataContext.Mode == EntityViewModelWrapper.ModelMode.Venue)
            {
                ImageBackground.ShowSubtitleContentCommand = NavigateAddressesCommand;
            }
        }

        private void DisposeHeaderImage()
        {
            ImageBackground.ShowImageFullscreenCommand = null;
            ImageBackground.ShowSubtitleContentCommand = null;

            ImageBackground.Dispose();
            ImagePlaceholder.Content = null;
        }

        private void InitializeStyle()
        {
            PlaceholderSeparator.Color = Theme.EntitySeparator;

            DescriptionLabel.Font = Theme.EntityDescrFont;
            DescriptionLabel.TextColor = Theme.CellText;
        }

        private void InitializeBottomGradientState()
        {
            _bottomGradient = new CAGradientLayer
                {
                    Frame = BottomGradientView.Bounds,
                    Colors = new [] { 
                        Theme.TextGradient.ColorWithAlpha(0.2f).CGColor, 
                        Theme.TextGradient.CGColor 
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
                        ImageHeight;

                _bottomGradient.Hidden = textHeight <= labelHeight;
            }
            else
            {
                _bottomGradient.Hidden = true;
            }
        }
    }
}