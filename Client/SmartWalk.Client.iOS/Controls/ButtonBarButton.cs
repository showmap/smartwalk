using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Client.iOS.Resources;
using SmartWalk.Client.iOS.Utils;

namespace SmartWalk.Client.iOS.Controls
{
    [Register("ButtonBarButton")]
    public class ButtonBarButton : UIButton
    {
        public static readonly SizeF DefaultVerticalSize = new SizeF(44, 44);
        public static readonly SizeF DefaultLandscapeSize = new SizeF(33, 33);

        private UIImageView _imageView;
        private bool _isSemiTransparent;

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
            bool isSemiTransparent = false) 
            : this(
                verticalIcon,
                landscapeIcon,
                DefaultVerticalSize,
                DefaultLandscapeSize,
                isSemiTransparent) 
        {
        }

        public ButtonBarButton(
            UIImage verticalIcon,
            UIImage landscapeIcon,
            SizeF? verticalSize,
            SizeF? landscapeSize,
            bool isSemiTransparent = false)
                : base(UIButtonType.Custom)
        {
            Initialize(
                verticalIcon, 
                landscapeIcon,
                verticalSize,
                landscapeSize,
                isSemiTransparent);
            UpdateState();
        }

        public UIImage VerticalIcon { get; set; }
        public UIImage LandscapeIcon { get; set; }
        public SizeF VerticalSize { get; set; }
        public SizeF LandscapeSize { get; set; }

        public bool IsSemiTransparent
        {
            get
            {
                return _isSemiTransparent;
            }
            set
            {
                if (_isSemiTransparent != value)
                {
                    _isSemiTransparent = value;

                    if (_isSemiTransparent)
                    {
                        SetBackgroundImage(Theme.SemiTransImage, UIControlState.Normal);
                    }
                    else
                    {
                        SetBackgroundImage(null, UIControlState.Normal);
                    }
                }
            }
        }

        public void UpdateState()
        {
            var frame = Frame;

            if (ScreenUtil.IsVerticalOrientation)
            {
                Frame = new RectangleF(frame.Location, VerticalSize);
                _imageView.Frame = new RectangleF(PointF.Empty, VerticalSize);
                _imageView.Image = VerticalIcon;
            }
            else
            {
                Frame = new RectangleF(frame.Location, LandscapeSize);
                _imageView.Frame = new RectangleF(PointF.Empty, LandscapeSize);
                _imageView.Image = LandscapeIcon ?? VerticalIcon;
            }
        }

        private void Initialize(
            UIImage verticalIcon,
            UIImage landscapeIcon,
            SizeF? verticalSize,
            SizeF? landscapeSize,
            bool isSemiTransparent = false)
        {
            VerticalIcon = verticalIcon;
            LandscapeIcon = landscapeIcon;
            VerticalSize = verticalSize ?? DefaultVerticalSize;
            LandscapeSize = landscapeSize ?? DefaultLandscapeSize;
            IsSemiTransparent = isSemiTransparent;

            SetBackgroundImage(Theme.BlackImage, UIControlState.Highlighted);

            _imageView = new UIImageView();
            AddSubview(_imageView);
        }
    }
}