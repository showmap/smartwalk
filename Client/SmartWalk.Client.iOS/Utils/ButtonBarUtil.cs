using System;
using System.Drawing;
using MonoTouch.UIKit;
using SmartWalk.Client.iOS.Resources;
using SmartWalk.Client.iOS.Controls;
using System.Linq;

namespace SmartWalk.Client.iOS.Utils
{
    public static class ButtonBarUtil
    {
        public static UIBarButtonItem CreateGapSpacer(float width = 0f)
        {
            var spacer = new UIBarButtonItem(UIBarButtonSystemItem.FixedSpace);
            spacer.Width = (int)width == 0 ? Theme.NavBarPaddingCompensate : width;
            return spacer;
        }

        public static ButtonBarButton Create(
            UIImage verticalIcon,
            UIImage landscapeIcon,
            bool isSemiTransparent = false)
        {
            var button = new ButtonBarButton(verticalIcon, landscapeIcon, isSemiTransparent);
            return button;
        }

        public static ButtonBarButton Create(
            UIImage verticalIcon,
            UIImage landscapeIcon,
            SizeF? verticalSize,
            SizeF? landscapeSize,
            bool isSemiTransparent = false)
        {
            var button = new ButtonBarButton(
                verticalIcon,
                landscapeIcon,
                verticalSize,
                landscapeSize,
                isSemiTransparent);
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

            navItem.SetLeftBarButtonItems(new [] { CreateGapSpacer(), barButton }, true);
        }

        public static void UpdateButtonsFrameOnRotation(UIBarButtonItem[] items)
        {
            if (items == null) return;

            var buttons = items
                .Select(i => i.CustomView as ButtonBarButton)
                .Where(b => b != null)
                .ToArray();

            UpdateButtonsFrameOnRotation(buttons);
        }

        public static void UpdateButtonsFrameOnRotation(ButtonBarButton[] buttons)
        {
            if (buttons == null) return;

            foreach (var button in buttons)
            {
                button.UpdateState();
            }
        }
    }
}