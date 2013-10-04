using System;
using System.Drawing;
using MonoTouch.UIKit;
using SmartWalk.iOS.Resources;

namespace SmartWalk.iOS.Utils
{
    public static class ButtonBarUtil
    {
        public const int SpacerWidth = -5;

        public static UIBarButtonItem CreateSpacer()
        {
            var spacer = new UIBarButtonItem(UIBarButtonSystemItem.FixedSpace);
            spacer.Width = SpacerWidth;
            return spacer;
        }

        public static UIButton Create(UIImage icon)
        {
            return Create(icon, new SizeF(44, 44));
        }

        public static UIButton Create(UIImage icon, SizeF size)
        {
            var button = new UIButton(UIButtonType.Custom);
            Initialize(button, icon, size);
            return button;
        }

        public static void Initialize(UIButton button, UIImage icon)
        {
            Initialize(button, icon, new SizeF(44, 44));
        }

        public static void Initialize(UIButton button, UIImage icon, SizeF size)
        {
            button.Frame = new RectangleF(PointF.Empty, size);
            button.AddSubview(new UIImageView(icon));
            button.SetBackgroundImage(Theme.BlackImage, UIControlState.Highlighted);
        }

        public static void OverrideNavigatorBackButton(
            UINavigationItem navItem,
            UINavigationController navController)
        {
            navItem.HidesBackButton = true;

            var spacer = CreateSpacer();

            var button = Create(ThemeIcons.NavBarBackIcon);
            button.TouchUpInside += (sender, e) => 
                navController.PopViewControllerAnimated(true);
            var barButton = new UIBarButtonItem(button);

            navItem.SetLeftBarButtonItems(new [] { spacer, barButton }, true);
        }

        public static UIBarButtonItem[] GetUpDownBarItems(Action upClickHandler, Action downClickHandler)
        {
            var spacer = ButtonBarUtil.CreateSpacer();

            var buttonUp = ButtonBarUtil.Create(ThemeIcons.NavBarUpIcon, new SizeF(34, 44));
            buttonUp.TouchUpInside += (s, e) => upClickHandler();
            var barButtonUp = new UIBarButtonItem(buttonUp) { Width = 34 };

            var buttonDown = ButtonBarUtil.Create(ThemeIcons.NavBarDownIcon, new SizeF(34, 44));
            buttonDown.TouchUpInside += (s, e) => downClickHandler();
            var barButtonDown = new UIBarButtonItem(buttonDown) { Width = 34 };

            var result = new [] { spacer, barButtonDown, spacer, barButtonUp };
            return result;
        }
    }
}