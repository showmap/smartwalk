using System.Drawing;
using MonoTouch.UIKit;

namespace SmartWalk.Client.iOS.Resources
{
    public static class Theme
    {
        private const string HelveticaBold = "HelveticaNeue-Bold";
        private const string HelveticaMedium = "HelveticaNeue-Medium";
        private const string HelveticaRegular = "HelveticaNeue";
        private const string HelveticaLight = "HelveticaNeue-Light";

        public static readonly UIImage HighlightImage = UIImage.FromFile("Images/Black.png");
        public static readonly UIImage TransImage = UIImage.FromFile("Images/Trans.png");
        public static readonly UIImage SemiTransImage = UIImage.FromFile("Images/SemiTrans.png");
        public static readonly UIImage SemiTransWhiteImage = UIImage.FromFile("Images/SemiTransWhite.png");

        public static readonly UIColor IconPassive = UIColor.FromRGB(160, 160, 160);
        public static readonly UIColor IconVeryPassive = UIColor.FromRGB(205, 205, 205);
        public static readonly UIColor IconActive = UIColor.FromRGB(143, 44, 250);

        public static readonly UIColor NavBarBackground = UIColor.FromRGBA(100, 31, 174, 180);
        public static readonly UIColor NavBarText = UIColor.White;
        public static readonly UIColor NavBarLightText = UIColor.FromRGB(62, 24, 107);
        public static readonly UIFont NavBarFont = UIFont.FromName(HelveticaBold, 15);
        public static int NavBarPaddingCompensate = -9;
        public static int ToolBarPaddingCompensate = -16;

        public static readonly UIColor ViewBlack = UIColor.FromRGB(8, 8, 8);
        public static readonly UIFont NoDataFont = UIFont.FromName(HelveticaRegular, 16);
        public static readonly UIFont LoadingFont = UIFont.FromName(HelveticaRegular, 16);
        public static readonly UIColor LoadingText = UIColor.FromRGB(170, 170, 170);
        public static readonly UIFont HomeHeaderFont = UIFont.FromName(HelveticaBold, 15);
        public static readonly UIFont ActionSheetFont = UIFont.FromName(HelveticaBold, 17);
        public static readonly UIFont ActionSheetCancelFont = UIFont.FromName(HelveticaBold, 18);

        public static readonly UIColor RefreshControl = UIColor.FromRGB(200, 200, 200);
        public static readonly UIColor SearchControl = UIColor.FromRGB(230, 230, 230);

        public static readonly UIColor CellBackground = UIColor.White;
        public static readonly UIColor CellHighlight = UIColor.FromRGB(190, 190, 190);
        public static readonly UIColor CellSemiHighlight = UIColor.FromRGB(255, 255, 250);
        public static readonly UIColor CellSeparator = UIColor.FromRGB(234, 234, 234);
        public static readonly UIColor GroupCellBackground = UIColor.FromRGBA(255, 255, 255, 250);
        public static readonly UIColor HeaderCellBackground = UIColor.FromRGBA(251, 251, 251, 250);

        public static readonly UIColor CellText = UIColor.Black;
        public static readonly UIColor CellTextPassive = UIColor.FromRGB(153, 153, 153);
        public static readonly UIFont GroupHeaderTextFont = UIFont.FromName(HelveticaMedium, 13);
        public static readonly UIColor HeaderText = UIColor.FromRGB(88, 226, 194);
        public static readonly UIColor HyperlinkText = IconActive;

        public static readonly UIFont ButtonTextFont = UIFont.FromName(HelveticaLight, 15);
        public static readonly UIFont ButtonTextLandscapeFont = UIFont.FromName(HelveticaLight, 13);
        public static readonly UIFont SegmentsTextFont = UIFont.FromName(HelveticaLight, 14);
        public static readonly UIFont ContactTitleTextFont = UIFont.FromName(HelveticaLight, 14);
        public static readonly UIFont ContactTextFont = UIFont.FromName(HelveticaRegular, 16);

        public static readonly UIFont BackgroundImageTitleTextFont = UIFont.FromName(HelveticaBold, 19);
        public static readonly UIFont BackgroundImageSubtitleTextFont = UIFont.FromName(HelveticaMedium, 16);
        public static readonly UIColor BackgroundImageTitleText = UIColor.White;
        public static readonly UIColor BackgroundImageSubtitleText = UIColor.FromRGB(255, 215, 51);
        public static readonly UIColor ImageGradient = UIColor.FromRGB(0, 0, 0);
        public static readonly UIColor DialogOutside = UIColor.FromRGBA(0, 0, 0, 200);

        public static readonly UIFont OrgEventDayFont = UIFont.FromName(HelveticaLight, 18);
        public static readonly UIFont OrgEventDayLandscapeFont = UIFont.FromName(HelveticaLight, 15);
        public static readonly UIFont OrgEventTwoDaysFont = UIFont.FromName(HelveticaLight, 14);
        public static readonly UIFont OrgEventTwoDaysLandscapeFont = UIFont.FromName(HelveticaLight, 11.5f);
        public static readonly UIFont OrgEventMonthFont = UIFont.FromName(HelveticaBold, 10);
        public static readonly UIFont OrgEventMonthLandscapeFont = UIFont.FromName(HelveticaBold, 8);
        public static readonly UIFont OrgEventTitleFont = UIFont.FromName(HelveticaRegular, 16);

        public static readonly UIColor EntityDescription = UIColor.FromRGB(135, 135, 135);
        public static readonly UIFont EntityDescriptionFont = UIFont.FromName(HelveticaLight, 16);
        public static readonly UIColor MapCell = UIColor.FromRGB(255, 248, 218);
        public static readonly UIColor MapCellAddress = UIColor.FromRGB(38, 38, 38);
        public static readonly UIFont MapCellAddressFont = UIFont.FromName(HelveticaLight, 16);
        public static readonly UIFont NextEntityFont = UIFont.FromName(HelveticaLight, 15);

        public static readonly UIFont OrgEventHeaderFont = UIFont.FromName(HelveticaLight, 16);
        public static readonly UIFont VenueCellThumbLabelFont = UIFont.FromName(HelveticaLight, 17);
        public static readonly UIFont VenueCellTitleFont = UIFont.FromName(HelveticaRegular, 16);
        public static readonly UIFont VenueCellAddressFont = UIFont.FromName(HelveticaLight, 14);
        public static readonly UIFont VenueShowCellTimeFont = UIFont.FromName(HelveticaRegular, 14);
        public static readonly UIFont VenueShowCellFinishedTimeFont = UIFont.FromName(HelveticaLight, 14);
        public static readonly UIFont VenueShowCellEndTimeFont = UIFont.FromName(HelveticaRegular, 11);
        public static readonly UIFont VenueShowCellFinishedEndTimeFont = UIFont.FromName(HelveticaLight, 11);
        public static readonly UIFont VenueShowCellFont = UIFont.FromName(HelveticaRegular, 16);
        public static readonly UIFont VenueShowCellFinishedFont = UIFont.FromName(HelveticaLight, 16);
        public static readonly UIFont VenueShowDescriptionCellFont = UIFont.FromName(HelveticaLight, 14);
        public static readonly UIFont VenueShowDetailsCellFont = UIFont.FromName(HelveticaLight, 15);

        public static readonly UIColor MapPinText = UIColor.White;
        public static readonly UIFont MapPinFont = UIFont.FromName(HelveticaLight, 12);
        public static readonly PointF MapPinOffset = new PointF(0, -16);
        public static readonly PointF MapPinTextOffset = new PointF(1, 0);

        public static readonly SizeF DefaultImageSize = new SizeF(10, 10);
        public static readonly SizeF ErrorImageSize = new SizeF(20, 20);
        public static readonly string DefaultImagePath = "res:Images/DefaultImage.png";
        public static readonly string ErrorImagePath = "res:Images/ErrorImage.png";

        public static void Apply()
        {
            UINavigationBar.Appearance.BarTintColor = NavBarBackground;
            UINavigationBar.Appearance.TintColor = Theme.NavBarText;

            UINavigationBar.Appearance.SetTitleTextAttributes(new UITextAttributes { 
                Font = Theme.NavBarFont,
                TextColor = Theme.NavBarText,
                TextShadowColor = UIColor.Clear,
                TextShadowOffset = new UIOffset(0, 0)
            });

            UIBarButtonItem.Appearance
                .SetBackButtonTitlePositionAdjustment(
                    new UIOffset(0, -64), UIBarMetrics.Default);

            UIToolbar.Appearance.TintColor = Theme.NavBarText;

            UISwitch.Appearance.OnTintColor = Theme.IconActive;
            UISwitch.Appearance.TintColor = Theme.IconActive;

            UISegmentedControl.Appearance.SetTitleTextAttributes(
                new UITextAttributes {
                    Font = SegmentsTextFont, 
                    TextColor = UIColor.White 
                }, 
                UIControlState.Highlighted);
            UISegmentedControl.Appearance.SetTitleTextAttributes(
                new UITextAttributes {
                    Font = SegmentsTextFont,
                    TextColor = Theme.IconActive 
                }, 
                UIControlState.Normal);
            UISegmentedControl.Appearance.TintColor = Theme.IconActive;

            var buttonLabelAp = UILabel.AppearanceWhenContainedIn(typeof(UIButton));
            buttonLabelAp.Font = ButtonTextFont;
            buttonLabelAp.TextColor = CellText;
        }
    }
}