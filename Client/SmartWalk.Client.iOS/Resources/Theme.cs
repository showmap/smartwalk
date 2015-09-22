using System;
using CoreGraphics;
using UIKit;

namespace SmartWalk.Client.iOS.Resources
{
    public static class Theme
    {
        public static int NavBarPaddingCompensate = -9;
        public static int ToolBarPaddingCompensate = -16;

        public static readonly UIFont NavBarFont = SystemFontOfSize(15, FontWeight.Bold);
        public static readonly UIFont ContentFont = SystemFontOfSize(16, FontWeight.Regular);
        public static readonly UIFont ActionSheetFont = SystemFontOfSize(17, FontWeight.Bold);
        public static readonly UIFont ActionSheetCancelFont = SystemFontOfSize(18, FontWeight.Bold);
        public static readonly UIFont ButtonTextFont = SystemFontOfSize(15, FontWeight.Light);

        public static readonly UIFont GroupHeaderTextFont = SystemFontOfSize(13, FontWeight.Medium);
        public static readonly UIFont ContactTitleTextFont = SystemFontOfSize(14, FontWeight.Light);

        public static readonly UIFont BackgroundImageTitleTextFont = SystemFontOfSize(19, FontWeight.Bold);
        public static readonly UIFont BackgroundImageSubtitleTextFont = SystemFontOfSize(16, FontWeight.Medium);

        public static readonly UIFont HomeHeaderFont = SystemFontOfSize(15, FontWeight.Bold);

        public static readonly UIFont OrgEventDayFont = SystemFontOfSize(18, FontWeight.Light);
        public static readonly UIFont OrgEventDayLandscapeFont = SystemFontOfSize(15, FontWeight.Light);
        public static readonly UIFont OrgEventTwoDaysFont = SystemFontOfSize(14, FontWeight.Light);
        public static readonly UIFont OrgEventTwoDaysLandscapeFont = SystemFontOfSize(11.5f, FontWeight.Light);
        public static readonly UIFont OrgEventMonthFont = SystemFontOfSize(10, FontWeight.Bold);
        public static readonly UIFont OrgEventMonthLandscapeFont = SystemFontOfSize(8, FontWeight.Bold);

        public static readonly UIFont EntityDescriptionFont = SystemFontOfSize(16, FontWeight.Light);
        public static readonly UIFont NextEntityFont = SystemFontOfSize(16, FontWeight.Light);

        public static readonly UIFont OrgEventHeaderFont = SystemFontOfSize(14, FontWeight.Light);
        public static readonly UIFont VenueNameFont = SystemFontOfSize(14, FontWeight.Medium);
        public static readonly UIFont VenueAddressFont = SystemFontOfSize(14, FontWeight.Light);
        public static readonly UIFont VenueShowLogoFont = SystemFontOfSize(17, FontWeight.Light);
        public static readonly UIFont VenueShowTimeFont = SystemFontOfSize(14, FontWeight.Regular);
        public static readonly UIFont VenueShowFinishedTimeFont = SystemFontOfSize(14, FontWeight.Light);
        public static readonly UIFont VenueShowEndTimeFont = SystemFontOfSize(10, FontWeight.Regular);
        public static readonly UIFont VenueShowFinishedEndTimeFont = SystemFontOfSize(10, FontWeight.Light);
        public static readonly UIFont VenueShowFinishedFont = SystemFontOfSize(16, FontWeight.Light);
        public static readonly UIFont VenueShowDescriptionFont = SystemFontOfSize(14, FontWeight.Light);
        public static readonly UIFont VenueShowDetailsFont = SystemFontOfSize(15, FontWeight.Light);

        public static readonly UIFont MapPinFont = SystemFontOfSize(12, FontWeight.Regular);
        public static readonly CGPoint MapPinOffset = new CGPoint(0, -16);
        public static readonly CGPoint MapPinTextOffset = new CGPoint(1, 0);

        public static readonly CGSize DefaultImageSize = new CGSize(10, 10);
        public static readonly CGSize ErrorImageSize = new CGSize(20, 20);
        public static readonly string DefaultImagePath = "res:Images/DefaultImage.png";
        public static readonly string ErrorImagePath = "res:Images/ErrorImage.png";

        public static void Apply()
        {
            UINavigationBar.Appearance.BarTintColor = ThemeColors.HeaderBackground;
            UINavigationBar.Appearance.TintColor = ThemeColors.ContentDarkText;

            UINavigationBar.Appearance.SetTitleTextAttributes(
                new UITextAttributes { 
                    Font = Theme.NavBarFont,
                    TextColor = ThemeColors.ContentDarkText,
                    TextShadowColor = UIColor.Clear,
                    TextShadowOffset = new UIOffset(0, 0)
                });

            UIBarButtonItem.Appearance
                .SetBackButtonTitlePositionAdjustment(
                new UIOffset(0, -64), UIBarMetrics.Default);

            UIToolbar.Appearance.TintColor = ThemeColors.ContentDarkText;

            UISwitch.Appearance.OnTintColor = ThemeColors.Action;
            UISwitch.Appearance.TintColor = ThemeColors.Action;

            var buttonLabelAp = UILabel.AppearanceWhenContainedIn(typeof(UIButton));
            buttonLabelAp.Font = ButtonTextFont;
            buttonLabelAp.TextColor = ThemeColors.ContentLightText;
        }

        private static UIFont SystemFontOfSize(nfloat size, FontWeight weight)
        {
            if (UIDevice.CurrentDevice.CheckSystemVersion(8, 2))
            {
                return UIFont.SystemFontOfSize(size, GetUIFontWeight(weight));
            }
            else
            {
                return UIFont.FromName(GetFontName(weight), size);
            }
        }

        private static UIFontWeight GetUIFontWeight(FontWeight weight) 
        {
            switch (weight) 
            {
                case FontWeight.Bold:
                    return UIFontWeight.Bold;

                case FontWeight.Medium:
                    return UIFontWeight.Medium;

                case FontWeight.Light:
                    return UIFontWeight.Light;

                default:
                    return UIFontWeight.Regular;
            }
        }

        private static string GetFontName(FontWeight weight) 
        {
            switch (weight) 
            {
                case FontWeight.Bold:
                    return "HelveticaNeue-Bold";

                case FontWeight.Medium:
                    return "HelveticaNeue-Medium";

                case FontWeight.Light:
                    return "HelveticaNeue-Light";

                default:
                    return "HelveticaNeue";
            }
        }

        private enum FontWeight
        {
            Light,
            Regular,
            Medium,
            Bold
        }
    }
}