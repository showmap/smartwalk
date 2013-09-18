using System;
using System.Drawing;
using MonoTouch.UIKit;

namespace SmartWalk.iOS.Utils
{
    public static class ButtonBarUtil
    {
        public static UIBarButtonItem CreateSpacer()
        {
            var spacer = new UIBarButtonItem(UIBarButtonSystemItem.FixedSpace);
            spacer.Width = -5;
            return spacer;
        }

        public static UIButton Create(string iconFile)
        {
            return Create(iconFile, new SizeF(44, 44));
        }

        public static UIButton Create(string iconFile, SizeF size)
        {
            var button = new UIButton(UIButtonType.Custom);
            button.Frame = new RectangleF(PointF.Empty, size);
            button.AddSubview(new UIImageView(UIImage.FromFile(iconFile)));
            button.SetBackgroundImage(UIImage.FromFile("Images/Black.png"), UIControlState.Highlighted);
            return button;
        }

        public static UIBarButtonItem[] GetUpDownBarItems(Action upClickHandler, Action downClickHandler)
        {
            var spacer = ButtonBarUtil.CreateSpacer();

            var buttonUp = ButtonBarUtil.Create("Icons/NavBarUp.png", new SizeF(34, 44));
            buttonUp.TouchUpInside += (s, e) => upClickHandler();
            var barButtonUp = new UIBarButtonItem(buttonUp) { Width = 34 };

            var buttonDown = ButtonBarUtil.Create("Icons/NavBarDown.png", new SizeF(34, 44));
            buttonDown.TouchUpInside += (s, e) => downClickHandler();
            var barButtonDown = new UIBarButtonItem(buttonDown) { Width = 34 };

            var result = new [] { spacer, barButtonUp, spacer, barButtonDown };
            return result;
        }
    }
}