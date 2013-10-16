using System;
using System.Drawing;
using MonoTouch.UIKit;
using SmartWalk.iOS.Resources;
using SmartWalk.iOS.Controls;

namespace SmartWalk.iOS.Utils
{
    public static class ButtonBarUtil
    {
        public const int SpacerWidth = -5;

        public static UIBarButtonItem CreateSpacer(int width = SpacerWidth)
        {
            var spacer = new UIBarButtonItem(UIBarButtonSystemItem.FixedSpace);
            spacer.Width = SpacerWidth;
            return spacer;
        }

        public static ButtonBarButton Create(
            UIImage verticalIcon,
            UIImage landscapeIcon)
        {
            var button = new ButtonBarButton(verticalIcon, landscapeIcon);
            return button;
        }

        public static ButtonBarButton Create(
            UIImage verticalIcon,
            UIImage landscapeIcon,
            SizeF? verticalSize,
            SizeF? landscapeSize)
        {
            var button = new ButtonBarButton(
                verticalIcon,
                landscapeIcon,
                verticalSize,
                landscapeSize);
            return button;
        }

        public static void OverrideNavigatorBackButton(
            UINavigationItem navItem,
            Action backButtonClickHandler)
        {
            navItem.HidesBackButton = true;

            var button = Create(ThemeIcons.NavBarBack, ThemeIcons.NavBarBackLandscape);
            button.TouchUpInside += (sender, e) => backButtonClickHandler();
            var barButton = new UIBarButtonItem(button);

            navItem.SetLeftBarButtonItems(new [] { CreateSpacer(), barButton }, true);
        }

        public static void UpdateButtonsFrameOnRotation(UIBarButtonItem[] items)
        {
            if (items == null) return;

            foreach (var item in items)
            {
                var button = item.CustomView as ButtonBarButton;
                if (button != null)
                {
                    button.UpdateState();
                }
            }
        }

        public static UIBarButtonItem[] GetUpDownBarItems(Action upClickHandler, Action downClickHandler)
        {
            var buttonUp = ButtonBarUtil.Create(ThemeIcons.NavBarUp, null, new SizeF(34, 44), null);
            buttonUp.TouchUpInside += (s, e) => upClickHandler();
            var barButtonUp = new UIBarButtonItem(buttonUp);

            var buttonDown = ButtonBarUtil.Create(ThemeIcons.NavBarDown, null, new SizeF(34, 44), null);
            buttonDown.TouchUpInside += (s, e) => downClickHandler();
            var barButtonDown = new UIBarButtonItem(buttonDown);

            var result = new [] { CreateSpacer(), barButtonDown, barButtonUp };
            return result;
        }
    }
}