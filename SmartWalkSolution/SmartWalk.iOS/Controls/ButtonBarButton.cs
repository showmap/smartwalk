using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.iOS.Resources;
using SmartWalk.iOS.Utils;

namespace SmartWalk.iOS.Controls
{
    [Register("ButtonBarButton")]
    public class ButtonBarButton : UIButton
    {
        public static readonly SizeF DefaultVerticalSize = new SizeF(44, 44);
        public static readonly SizeF DefaultLandscapeSize = new SizeF(33, 33);

        private readonly UIImage _verticalIcon;
        private readonly UIImage _landscapeIcon;
        private readonly SizeF _verticalSize;
        private readonly SizeF _landscapeSize;

        private readonly UIImageView _imageView;

        public ButtonBarButton(
            UIImage verticalIcon,
            UIImage landscapeIcon) 
            : this(
                verticalIcon,
                landscapeIcon,
                DefaultVerticalSize,
                DefaultLandscapeSize) 
        {
        }

        public ButtonBarButton(
            UIImage verticalIcon,
            UIImage landscapeIcon,
            SizeF? verticalSize,
            SizeF? landscapeSize)
                : base(UIButtonType.Custom)
        {
            if (verticalIcon == null) throw new ArgumentNullException("verticalIcon");

            _verticalIcon = verticalIcon;
            _landscapeIcon = landscapeIcon;
            _verticalSize = verticalSize ?? DefaultVerticalSize;
            _landscapeSize = landscapeSize ?? DefaultLandscapeSize;

            SetBackgroundImage(Theme.BlackImage, UIControlState.Highlighted);

            _imageView = new UIImageView();
            AddSubview(_imageView);

            UpdateState();
        }

        public void UpdateState()
        {
            var frame = Frame;

            if (ScreenUtil.IsVerticalOrientation)
            {
                Frame = new RectangleF(frame.Location, _verticalSize);
                _imageView.Frame = new RectangleF(PointF.Empty, _verticalSize);
                _imageView.Image = _verticalIcon;
            }
            else
            {
                Frame = new RectangleF(frame.Location, _landscapeSize);
                _imageView.Frame = new RectangleF(PointF.Empty, _landscapeSize);
                _imageView.Image = _landscapeIcon ?? _verticalIcon;
            }
        }
    }
}