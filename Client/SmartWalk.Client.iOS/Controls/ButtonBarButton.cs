using System;
using CoreGraphics;
using Foundation;
using UIKit;
using SmartWalk.Client.iOS.Resources;
using SmartWalk.Client.iOS.Utils;

namespace SmartWalk.Client.iOS.Controls
{
    [Register("ButtonBarButton")]
    public class ButtonBarButton : UIButton
    {
        public static readonly CGSize DefaultVerticalSize = 
            new CGSize(
                UIConstants.ToolBarVerticalHeight,
                UIConstants.ToolBarVerticalHeight);
        public static readonly CGSize DefaultLandscapeSize = 
            new CGSize(
                UIConstants.ToolBarHorizontalHeight, 
                UIConstants.ToolBarHorizontalHeight);

        private UIImageView _iconImageView;
        private SemiTransparentType _semiTransparentType;

        private Circle _background;

        public ButtonBarButton(IntPtr handle) : base(handle)
        {
            Initialize(
                null, 
                null,
                DefaultVerticalSize,
                DefaultLandscapeSize);
            UpdateState();
        }

        public ButtonBarButton(
            UIImage verticalIcon,
            UIImage landscapeIcon,
            SemiTransparentType semiTransparentType = SemiTransparentType.None) 
            : this(
                verticalIcon,
                landscapeIcon,
                DefaultVerticalSize,
                DefaultLandscapeSize,
                semiTransparentType) 
        {
        }

        public ButtonBarButton(
            UIImage verticalIcon,
            UIImage landscapeIcon,
            CGSize? verticalSize,
            CGSize? landscapeSize,
            SemiTransparentType semiTransparentType = SemiTransparentType.None)
                : base()
        {
            Initialize(
                verticalIcon, 
                landscapeIcon,
                verticalSize,
                landscapeSize,
                semiTransparentType);
            UpdateState();
        }

        public UIImage VerticalIcon { get; set; }
        public UIImage LandscapeIcon { get; set; }
        public CGSize VerticalSize { get; set; }
        public CGSize LandscapeSize { get; set; }

        public SemiTransparentType SemiTransparentType
        {
            get
            {
                return _semiTransparentType;
            }
            set
            {
                if (_semiTransparentType != value)
                {
                    _semiTransparentType = value;
                    UpdateBackgroundState();
                }
            }
        }

        private UIImageView IconImageView
        {
            get
            {
                if (_iconImageView == null)
                {
                    _iconImageView = new UIImageView();
                    AddSubview(_iconImageView);
                }

                return _iconImageView;
            }
        }

        public void UpdateState()
        {
            var frame = Frame;

            if (ScreenUtil.IsVerticalOrientation)
            {
                Frame = new CGRect(frame.Location, VerticalSize);

                if (VerticalIcon != null || LandscapeIcon != null)
                {
                    IconImageView.Frame = new CGRect(CGPoint.Empty, VerticalSize);
                    IconImageView.Image = VerticalIcon;
                }
            }
            else
            {
                Frame = new CGRect(frame.Location, LandscapeSize);

                if (VerticalIcon != null || LandscapeIcon != null)
                {
                    IconImageView.Frame = new CGRect(CGPoint.Empty, LandscapeSize);
                    IconImageView.Image = LandscapeIcon ?? VerticalIcon;
                }
            }

            _background.Frame = Bounds;

            switch (SemiTransparentType)
            {
                case SemiTransparentType.Light:
                    IconImageView.TintColor = ThemeColors.ContentLightText;
                    break;

                case SemiTransparentType.Dark:
                    IconImageView.TintColor = ThemeColors.ContentDarkText;
                    break;

                case SemiTransparentType.None:
                    IconImageView.TintColor = ThemeColors.ContentDarkText;
                    break;
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            ConsoleUtil.LogDisposed(this);
        }

        private void Initialize(
            UIImage verticalIcon,
            UIImage landscapeIcon,
            CGSize? verticalSize,
            CGSize? landscapeSize,
            SemiTransparentType semiTransparentType = SemiTransparentType.None)
        {
            _background = new Circle {
                UserInteractionEnabled = false
            };

            _background.Layer.ShadowColor = ThemeColors.ContentDarkBackground.CGColor;
            _background.Layer.ShadowOpacity = 0.13f;
            _background.Layer.ShadowOffset = new CGSize(2, 2);

            Add(_background);

            VerticalIcon = verticalIcon;
            LandscapeIcon = landscapeIcon;
            VerticalSize = verticalSize ?? DefaultVerticalSize;
            LandscapeSize = landscapeSize ?? DefaultLandscapeSize;
            SemiTransparentType = semiTransparentType;

            TouchDown += (sender, e) => 
                _background.FillColor = ThemeColors.ContentLightHighlight.ColorWithAlpha(0.58f);
            TouchUpInside += (sender, e) => UpdateBackgroundState();
            TouchUpOutside += (sender, e) => UpdateBackgroundState();
        }

        private void UpdateBackgroundState()
        {
            switch (SemiTransparentType)
            {
                case SemiTransparentType.Light:
                    _background.FillColor = ThemeColors.PanelBackgroundAlpha;
                    _background.LineColor = ThemeColors.BorderDark.ColorWithAlpha(0.63f);
                    _background.LineWidth = ScreenUtil.HairLine;
                    break;

                case SemiTransparentType.Dark:
                    _background.FillColor = ThemeColors.ContentDarkBackground.ColorWithAlpha(0.35f);
                    _background.LineColor = UIColor.Clear;
                    _background.LineWidth = 0;
                    break;

                default:
                    _background.FillColor = UIColor.Clear;
                    _background.LineColor = UIColor.Clear;
                    _background.LineWidth = 0;
                    break;
            }
        }
    }

    public enum SemiTransparentType
    {
        None,
        Dark,
        Light
    }
}