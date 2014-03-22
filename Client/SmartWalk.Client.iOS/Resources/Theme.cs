using System.Drawing;
using MonoTouch.UIKit;
using SmartWalk.Client.iOS.Views.Common;
using SmartWalk.Client.iOS.Views.Common.EntityCell;
using SmartWalk.Client.iOS.Views.HomeView;
using SmartWalk.Client.iOS.Views.OrgEventView;

namespace SmartWalk.Client.iOS.Resources
{
    public static class Theme
    {
        private const string HelveticaBold = "HelveticaNeue-Bold";
        private const string HelveticaMedium = "HelveticaNeue-Medium";
        private const string HelveticaRegular = "HelveticaNeue";
        private const string HelveticaLight = "HelveticaNeue-Light";

        public static readonly UIImage NavBarBackgroundImage = 
            UIImage.FromFile("Images/NavBarBackground.png");
        public static readonly UIImage NavBarLandscapeBackgroundImage = 
            UIImage.FromFile("Images/NavBarLandscapeBackground.png");
        public static readonly UIImage ShadowImage = UIImage.FromFile("Images/Shadow.png")
            .CreateResizableImage(new UIEdgeInsets(0, 1, 0, 1));
        public static readonly UIColor BackgroundPatternColor = UIColor.FromPatternImage(
            UIImage.FromFile("Images/Background.png"));
        public static readonly UIImage BlackImage = UIImage.FromFile("Images/Black.png");

        public static readonly UIColor NavBarBackground = UIColor.FromRGB(51, 51, 51);
        public static readonly UIColor NavBarBackgroundiOS7 = UIColor.FromRGB(16, 16, 16);
        public static readonly UIColor NavBarText = UIColor.White;
        public static readonly UIFont NavBarFont = UIFont.FromName(HelveticaBold, 15);
        public static int NavBarPaddingCompensate = -5;
        public static int ToolBarPaddingCompensate = -12;

        public static readonly UIColor RefreshControl = UIColor.FromRGB(200, 200, 200);
        public static readonly UIColor TextGradient = UIColor.FromRGB(240, 240, 240);

        public static readonly UIColor CellBackground = UIColor.White;
        public static readonly UIColor CellHighlight = UIColor.FromRGB(255, 216, 0);
        public static readonly UIColor CellSeparator = UIColor.FromRGB(220, 220, 220);
        public static readonly UIColor HeaderCellBackground = UIColor.FromRGBA(255, 255, 255, 250);
        public static readonly UIColor ToolBarButtonHighlightedBackground = 
            UIColor.FromRGBA(255, 255, 255, 230);

        public static readonly UIColor CellText = UIColor.Black;
        public static readonly UIColor CellTextPassive = UIColor.FromRGB(153, 153, 153);
        public static readonly UIColor CellTextHint = UIColor.FromRGB(187, 187, 187);
        public static readonly UIColor CellTextHighlight = UIColor.White;
        public static readonly UIColor HyperlinkText = UIColor.FromRGB(74, 197, 144);

        public static readonly UIFont ToolBarButtonTextFont = UIFont.FromName(HelveticaLight, 12);
        public static readonly UIFont ButtonTextFont = UIFont.FromName(HelveticaLight, 15);
        public static readonly UIFont ButtonTextLandscapeFont = UIFont.FromName(HelveticaLight, 13);
        public static readonly UIFont ContactTitleTextFont = UIFont.FromName(HelveticaLight, 14);
        public static readonly UIFont ContactTextFont = UIFont.FromName(HelveticaRegular, 16);

        public static readonly UIFont OrgTextFont = UIFont.FromName(HelveticaRegular, 19);
        public static readonly UIFont GroupHeaderTextFont = UIFont.FromName(HelveticaMedium, 13);

        public static readonly UIFont OrgEventDayFont = UIFont.FromName(HelveticaLight, 18);
        public static readonly UIFont OrgEventWeekDayFont = UIFont.FromName(HelveticaBold, 10);
        public static readonly UIFont OrgEventDateFont = UIFont.FromName(HelveticaLight, 17);
        public static readonly UIFont OrgEventHintFont = UIFont.FromName(HelveticaLight, 15);
        public static readonly UIColor OrgEventActive = CellHighlight;
        public static readonly UIColor OrgEventPassive = UIColor.FromRGB(187, 187, 187);

        public static readonly UIColor EntitySeparator = UIColor.FromRGBA(120, 120, 120, 127);
        public static readonly UIFont EntityDescrFont = UIFont.FromName(HelveticaRegular, 15);
        public const int EntityDescrTextLineHeight = 19;

        public static readonly UIFont MapViewAddressFont = UIFont.FromName(HelveticaLight, 14);

        public static readonly UIFont OrgEventHeaderFont = UIFont.FromName(HelveticaLight, 16);
        public static readonly UIFont VenueCellTitleFont = UIFont.FromName(HelveticaMedium, 15);
        public static readonly UIFont VenueCellAddressFont = UIFont.FromName(HelveticaLight, 14);

        public static readonly UIFont VenueShowCellTimeFont = UIFont.FromName(HelveticaRegular, 14);
        public static readonly UIFont VenueShowCellFinishedTimeFont = UIFont.FromName(HelveticaLight, 14);
        public static readonly UIFont VenueShowCellTimeAmPmFont = UIFont.FromName(HelveticaLight, 9);
        public static readonly UIFont VenueShowCellFont = UIFont.FromName(HelveticaRegular, 14);
        public const int VenueShowTextLineHeight = 17;

        public static readonly UIColor MapPinText = UIColor.White;
        public static readonly UIFont MapPinFont = UIFont.FromName(HelveticaMedium, 14);
        public static readonly PointF MapPinOffset = new PointF(-8, -4);
        public static readonly PointF MapPinTextOffset = new PointF(5, 5);

        public static void Apply()
        {
            if (UIDevice.CurrentDevice.CheckSystemVersion(7, 0))
            {
                NavBarPaddingCompensate = -16;

                UIApplication.SharedApplication.SetStatusBarStyle(UIStatusBarStyle.LightContent, false);

                UINavigationBar.Appearance.BarTintColor = NavBarBackgroundiOS7;
            }
            else
            {
                UINavigationBar.Appearance.SetBackgroundImage(
                    NavBarBackgroundImage,
                    UIBarMetrics.Default);
                // TODO: bottom line has different color, bug?
                UINavigationBar.Appearance.SetBackgroundImage(
                    NavBarLandscapeBackgroundImage,
                    UIBarMetrics.LandscapePhone);

                UINavigationBar.Appearance.SetTitleVerticalPositionAdjustment(4, UIBarMetrics.Default);
                UINavigationBar.Appearance.SetTitleVerticalPositionAdjustment(2, UIBarMetrics.LandscapePhone);
            }

            UINavigationBar.Appearance.SetTitleTextAttributes(new UITextAttributes { 
                Font = Theme.NavBarFont,
                TextColor = Theme.NavBarText,
                TextShadowColor = UIColor.Clear,
                TextShadowOffset = new UIOffset(0, 0)
            });

            if (UIDevice.CurrentDevice.CheckSystemVersion(7, 0))
            {
                ToolBarPaddingCompensate = -16;

                UIToolbar.Appearance.BarTintColor = NavBarBackgroundiOS7;
            }
            else
            {
                UIToolbar.Appearance.SetBackgroundImage(
                    NavBarBackgroundImage,
                    UIToolbarPosition.Any,
                    UIBarMetrics.Default);
                UIToolbar.Appearance.SetBackgroundImage(
                    NavBarLandscapeBackgroundImage,
                    UIToolbarPosition.Any,
                    UIBarMetrics.LandscapePhone);
            }

            UISwitch.Appearance.OnTintColor = UIColor.DarkGray;
            UISwitch.Appearance.TintColor = UIColor.Gray;

            var buttonLabelAp = UILabel.AppearanceWhenContainedIn(typeof(UIButton));
            buttonLabelAp.Font = ButtonTextFont;
            buttonLabelAp.TextColor = CellText;

            var cellAp = UICollectionViewCell.AppearanceWhenContainedIn(typeof(HomeView));
            cellAp.BackgroundColor = Theme.CellBackground;

            var labelAp = UILabel.AppearanceWhenContainedIn(typeof(OrgCell));
            labelAp.Font = Theme.OrgTextFont;
            labelAp.TextColor = Theme.CellText;
            labelAp.HighlightedTextColor = Theme.CellTextHighlight;

            labelAp = UILabel.AppearanceWhenContainedIn(typeof(GroupHeaderCell));
            labelAp.Font = Theme.GroupHeaderTextFont;
            labelAp.TextColor = Theme.CellText;

            labelAp = UILabel.AppearanceWhenContainedIn(typeof(EntityCell));
            labelAp.Font = Theme.EntityDescrFont;
            labelAp.TextColor = Theme.CellText;

            labelAp = UILabel.AppearanceWhenContainedIn(typeof(OrgEventHeaderView));
            labelAp.Font = Theme.OrgEventHeaderFont;
            labelAp.TextColor = Theme.CellTextPassive;
        }
    }
}