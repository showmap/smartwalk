using CoreGraphics;
using UIKit;

namespace SmartWalk.Client.iOS.Resources
{
    public static class Theme
    {
        private const string HelveticaBold = "HelveticaNeue-Bold";
        private const string HelveticaMedium = "HelveticaNeue-Medium";
        private const string HelveticaRegular = "HelveticaNeue";
        private const string HelveticaLight = "HelveticaNeue-Light";

        public static int NavBarPaddingCompensate = -9;
        public static int ToolBarPaddingCompensate = -16;

        public static readonly UIFont NavBarFont = UIFont.FromName(HelveticaBold, 15);
        public static readonly UIFont ContentFont = UIFont.FromName(HelveticaRegular, 16);
        public static readonly UIFont ActionSheetFont = UIFont.FromName(HelveticaBold, 17);
        public static readonly UIFont ActionSheetCancelFont = UIFont.FromName(HelveticaBold, 18);
        public static readonly UIFont ButtonTextFont = UIFont.FromName(HelveticaLight, 15);

        public static readonly UIFont GroupHeaderTextFont = UIFont.FromName(HelveticaMedium, 13);
        public static readonly UIFont ContactTitleTextFont = UIFont.FromName(HelveticaLight, 14);

        public static readonly UIFont BackgroundImageTitleTextFont = UIFont.FromName(HelveticaBold, 19);
        public static readonly UIFont BackgroundImageSubtitleTextFont = UIFont.FromName(HelveticaMedium, 16);

        public static readonly UIFont HomeHeaderFont = UIFont.FromName(HelveticaBold, 15);

        public static readonly UIFont OrgEventDayFont = UIFont.FromName(HelveticaLight, 18);
        public static readonly UIFont OrgEventDayLandscapeFont = UIFont.FromName(HelveticaLight, 15);
        public static readonly UIFont OrgEventTwoDaysFont = UIFont.FromName(HelveticaLight, 14);
        public static readonly UIFont OrgEventTwoDaysLandscapeFont = UIFont.FromName(HelveticaLight, 11.5f);
        public static readonly UIFont OrgEventMonthFont = UIFont.FromName(HelveticaBold, 10);
        public static readonly UIFont OrgEventMonthLandscapeFont = UIFont.FromName(HelveticaBold, 8);

        public static readonly UIFont EntityDescriptionFont = UIFont.FromName(HelveticaLight, 16);
        public static readonly UIFont NextEntityFont = UIFont.FromName(HelveticaLight, 16);

        public static readonly UIFont OrgEventHeaderFont = UIFont.FromName(HelveticaLight, 14);
        public static readonly UIFont VenueNameFont = UIFont.FromName(HelveticaMedium, 14);
        public static readonly UIFont VenueAddressFont = UIFont.FromName(HelveticaLight, 14);
        public static readonly UIFont VenueShowLogoFont = UIFont.FromName(HelveticaLight, 17);
        public static readonly UIFont VenueShowTimeFont = UIFont.FromName(HelveticaRegular, 14);
        public static readonly UIFont VenueShowFinishedTimeFont = UIFont.FromName(HelveticaLight, 14);
        public static readonly UIFont VenueShowEndTimeFont = UIFont.FromName(HelveticaRegular, 10);
        public static readonly UIFont VenueShowFinishedEndTimeFont = UIFont.FromName(HelveticaLight, 10);
        public static readonly UIFont VenueShowFinishedFont = UIFont.FromName(HelveticaLight, 16);
        public static readonly UIFont VenueShowDescriptionFont = UIFont.FromName(HelveticaLight, 14);
        public static readonly UIFont VenueShowDetailsFont = UIFont.FromName(HelveticaLight, 15);

        public static readonly UIFont MapPinFont = UIFont.FromName(HelveticaRegular, 12);
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
    }
}