using System;
using System.Drawing;
using MonoTouch.UIKit;
using SmartWalk.Client.iOS.Resources;
using SmartWalk.Client.iOS.Controls;

namespace SmartWalk.Client.iOS.Utils
{
    public static class ButtonBarUtil
    {
        public static UIBarButtonItem CreateSpacer()
        {
            var spacer = new UIBarButtonItem(UIBarButtonSystemItem.FixedSpace);
            spacer.Width = Theme.NavBarPaddingCompensate;
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
    }
}