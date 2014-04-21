using System;
using MonoTouch.UIKit;
using SmartWalk.Client.iOS.Resources;

namespace SmartWalk.Client.iOS.Utils
{
    public static class ActionSheetUtil
    {
        public static UIActionSheet CreateActionSheet(EventHandler<UIButtonEventArgs> clickHandler)
        {
            var result = new UIActionSheet();
            result.Clicked += clickHandler;
            result.Style = UIActionSheetStyle.BlackTranslucent;
            result.WillPresent += OnWillPresent;

            return result;
        }

        private static void OnWillPresent(object sender, EventArgs e)
        {
            var actionSheet = (UIActionSheet)sender;
            actionSheet.WillPresent -= OnWillPresent;

            foreach (var subview in actionSheet.Subviews)
            {
                var button = subview as UIButton;
                if (button != null)
                {
                    button.SetTitleColor(Theme.ActionSheetText, UIControlState.Normal);
                    button.SetTitleColor(Theme.ActionSheetText, UIControlState.Selected);
                    button.SetTitleColor(Theme.ActionSheetText, UIControlState.Highlighted);
                    button.Font = Theme.ActionSheetFont;
                }
            }
        }
    }
}