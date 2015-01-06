using System.Drawing;
using System.Linq;
using MonoTouch.UIKit;
using SmartWalk.Client.iOS.Controls;
using SmartWalk.Client.iOS.Resources;

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
            SemiTransparentType semiTransparentType = SemiTransparentType.None)
        {
            var button = new ButtonBarButton(null, null, semiTransparentType);
            return button;
        }

        public static ButtonBarButton Create(
            UIImage verticalIcon,
            UIImage landscapeIcon,
            SemiTransparentType semiTransparentType = SemiTransparentType.None)
        {
            var button = 
                new ButtonBarButton(
                    verticalIcon, 
                    landscapeIcon, 
                    semiTransparentType);
            return button;
        }

        public static ButtonBarButton Create(
            UIImage verticalIcon,
            UIImage landscapeIcon,
            SizeF? verticalSize,
            SizeF? landscapeSize,
            SemiTransparentType semiTransparentType = SemiTransparentType.None)
        {
            var button = new ButtonBarButton(
                verticalIcon,
                landscapeIcon,
                verticalSize,
                landscapeSize,
                semiTransparentType);
            return button;
        }

        public static void UpdateButtonsFrameOnRotation(UIBarButtonItem[] items)
        {
            if (items == null) return;

            var buttons = GetButtonsFromBarItems(items);
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

        public static ButtonBarButton[] GetButtonsFromBarItems(UIBarButtonItem[] items)
        {
            var result = items
                .Select(i => i.CustomView as ButtonBarButton)
                .Where(b => b != null)
                .ToArray();
            return result;
        }

        public static UIView[] GetViewsFromBarItems(UIBarButtonItem[] items)
        {
            var result = items
                .Select(i => i.CustomView)
                .Where(b => b != null)
                .ToArray();
            return result;
        }

        public static UIBarButtonItem[] GetNavItemBarItems(this UINavigationItem navItem)
        {
            var leftItems = 
                navItem != null &&
                navItem.LeftBarButtonItems != null
                    ? navItem.LeftBarButtonItems
                    : Enumerable.Empty<UIBarButtonItem>();

            var rightItems = 
                navItem != null &&
                navItem.RightBarButtonItems != null
                    ? navItem.RightBarButtonItems
                    : Enumerable.Empty<UIBarButtonItem>();

            var result = leftItems.Union(rightItems).ToArray();
            return result;
        }

        public static UIBarButtonItem[] GetNavBarItems(this UINavigationBar navBar)
        {
            var result = navBar.Items
                .SelectMany(navItem => navItem.GetNavItemBarItems())
                .ToArray();
            return result;
        }
    }
}