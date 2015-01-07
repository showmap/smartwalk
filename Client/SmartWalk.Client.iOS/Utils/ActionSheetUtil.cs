using System;
using MonoTouch.UIKit;
using SmartWalk.Client.Core.Resources;
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

        public static UIAlertController CreateActionSheet()
        {
            var result = UIAlertController.Create(null, null, 
                UIAlertControllerStyle.ActionSheet);

            result.View.TintColor = ThemeColors.ContentLightText;

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
                    button.SetTitleColor(ThemeColors.ContentLightText, UIControlState.Normal);
                    button.SetTitleColor(ThemeColors.ContentLightText, UIControlState.Selected);
                    button.SetTitleColor(ThemeColors.ContentLightText, UIControlState.Highlighted);

                    if (button.TitleLabel.Text == Localization.CancelButton)
                    {
                        button.Font = Theme.ActionSheetCancelFont;
                    }
                    else 
                    {
                        button.Font = Theme.ActionSheetFont;
                    }
                }
            }
        }
    }
}