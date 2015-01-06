using System;
using System.Drawing;
using MonoTouch.CoreAnimation;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Client.iOS.Resources;
using SmartWalk.Client.iOS.Utils;

namespace SmartWalk.Client.iOS.Controls
{
    [Register("ButtonBarButton")]
    public class ButtonBarButton : UIButton
    {
        public static readonly SizeF DefaultVerticalSize = 
            new SizeF(
                UIConstants.ToolBarVerticalHeight,
                UIConstants.ToolBarVerticalHeight);
        public static readonly SizeF DefaultLandscapeSize = 
            new SizeF(
                UIConstants.ToolBarHorizontalHeight, 
                UIConstants.ToolBarHorizontalHeight);

        private UIImageView _iconImageView;
        private SemiTransparentType _semiTransparent;

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
            SizeF? verticalSize,
            SizeF? landscapeSize,
            SemiTransparentType semiTransparentType = SemiTransparentType.None)
                : base(UIButtonType.Custom)
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
        public SizeF VerticalSize { get; set; }
        public SizeF LandscapeSize { get; set; }

        public SemiTransparentType SemiTransparentType
        {
            get
            {
                return _semiTransparent;
            }
            set
            {
                if (_semiTransparent != value)
                {
                    _semiTransparent = value;

                    switch (_semiTransparent)
                    {
                        case SemiTransparentType.Light:
                            SetBackgroundImage(Theme.SemiTransWhiteImage, UIControlState.Normal);
                            SetBackgroundImage(Theme.SemiTransWhiteImage, UIControlState.Disabled);
                            break;

                        case SemiTransparentType.Dark:
                            SetBackgroundImage(Theme.SemiTransImage, UIControlState.Normal);
                            SetBackgroundImage(Theme.SemiTransImage, UIControlState.Disabled);
                            break;

                        default:
                            SetBackgroundImage(null, UIControlState.Normal);
                            SetBackgroundImage(null, UIControlState.Disabled);
                            break;
                    }
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
                Frame = new RectangleF(frame.Location, VerticalSize);

                if (VerticalIcon != null || LandscapeIcon != null)
                {
                    IconImageView.Frame = new RectangleF(PointF.Empty, VerticalSize);
                    IconImageView.Image = VerticalIcon;
                }
            }
            else
            {
                Frame = new RectangleF(frame.Location, LandscapeSize);

                if (VerticalIcon != null || LandscapeIcon != null)
                {
                    IconImageView.Frame = new RectangleF(PointF.Empty, LandscapeSize);
                    IconImageView.Image = LandscapeIcon ?? VerticalIcon;
                }
            }

            switch (SemiTransparentType)
            {
                case SemiTransparentType.Light:
                    IconImageView.TintColor = Theme.NavBarLightText;
                    break;

                case SemiTransparentType.Dark:
                    IconImageView.TintColor = Theme.NavBarText;
                    break;

                case SemiTransparentType.None:
                    IconImageView.TintColor = Theme.NavBarText;
                    break;
            }

            UpdateMask();
        }

        private void Initialize(
            UIImage verticalIcon,
            UIImage landscapeIcon,
            SizeF? verticalSize,
            SizeF? landscapeSize,
            SemiTransparentType semiTransparentType = SemiTransparentType.None)
        {
            VerticalIcon = verticalIcon;
            LandscapeIcon = landscapeIcon;
            VerticalSize = verticalSize ?? DefaultVerticalSize;
            LandscapeSize = landscapeSize ?? DefaultLandscapeSize;
            SemiTransparentType = semiTransparentType;

            SetBackgroundImage(Theme.HighlightImage, UIControlState.Highlighted);
        }

        private void UpdateMask()
        {
            var path = UIBezierPath.FromOval(Bounds);
            var mask = new CAShapeLayer {
                Frame = Bounds,
                Path = path.CGPath
            };

            Layer.Mask = mask;
        }
    }

    public enum SemiTransparentType
    {
        None,
        Dark,
        Light
    }
}