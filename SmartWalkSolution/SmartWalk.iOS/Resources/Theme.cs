using System.Drawing;
using MonoTouch.UIKit;
using SmartWalk.iOS.Views.Common;
using SmartWalk.iOS.Views.Common.EntityCell;
using SmartWalk.iOS.Views.HomeView;
using SmartWalk.iOS.Views.OrgEventView;

namespace SmartWalk.iOS.Resources
{
    public class Theme
    {
        private const string BrandonBlack = "BrandonGrotesque-Black";
        private const string BrandonBold = "BrandonGrotesque-Bold";
        private const string BrandonRegular = "BrandonGrotesque-Regular";

        public static readonly UIImage NavBarBackgroundImage = UIImage.FromFile("Images/NavBarBackground.png");
        public static readonly UIImage NavBarLandscapeBackgroundImage = UIImage.FromFile("Images/NavBarLandscapeBackground.png");
        public static readonly UIImage ShadowImage = UIImage.FromFile("Images/Shadow.png")
            .CreateResizableImage(new UIEdgeInsets(0, 1, 0, 1));
        public static readonly UIColor BackgroundPatternColor = UIColor.FromPatternImage(UIImage.FromFile("Images/Background.png"));
        public static readonly UIImage BlackImage = UIImage.FromFile("Images/Black.png");

        public static readonly UIColor NavBarBackground = UIColor.FromRGB(51, 51, 51);
        public static readonly UIColor NavBarText = UIColor.White;
        public static readonly UIFont NavBarFont = UIFont.FromName(BrandonBlack, 14);

        public static readonly UIColor CellBackground = UIColor.White;
        public static readonly UIColor CellHighlight = UIColor.FromRGB(255, 216, 0);
        public static readonly UIColor CellText = UIColor.Black;
        public static readonly UIColor CellTextHighlight = UIColor.White;

        public static readonly UIFont OrgText = UIFont.FromName(BrandonRegular, 24);
        public const int OrgTextLineSpacing = -9;
        public static readonly UIFont GroupHeaderText = UIFont.FromName(BrandonBlack, 14);

        public static readonly UIColor OrgEventDayText = UIColor.White;
        public static readonly UIFont OrgEventDayFont = UIFont.FromName(BrandonRegular, 18);
        public static readonly UIFont OrgEventWeekDayFont = UIFont.FromName(BrandonBlack, 10);
        public static readonly UIColor OrgEventDateText = UIColor.FromRGB(102, 102, 102);
        public static readonly UIFont OrgEventDateFont = UIFont.FromName(BrandonRegular, 18);
        public static readonly UIColor OrgEventHintText = UIColor.FromRGB(187, 187, 187);
        public static readonly UIFont OrgEventHintFont = UIFont.FromName(BrandonRegular, 18);
        public static readonly UIColor OrgEventActive = CellHighlight;
        public static readonly UIColor OrgEventPassive = UIColor.FromRGB(187, 187, 187);

        public static readonly UIColor EntityDescrText = UIColor.FromRGB(102, 102, 102);
        public static readonly UIFont EntityDescrFont = UIFont.FromName(BrandonRegular, 16);
        public const int EntityDescrTextLineHeight = 24;

        public static readonly UIFont OrgEventHeaderFont = UIFont.FromName(BrandonRegular, 16);
        public static readonly UIColor OrgEventHeaderText = UIColor.FromRGB(153, 153, 153);
        public static readonly UIFont VenueCellTitleFont = UIFont.FromName(BrandonBold, 14);
        public static readonly UIColor VenueCellTitleText = UIColor.Black;
        public static readonly UIFont VenueCellAddressFont = UIFont.FromName(BrandonRegular, 14);
        public static readonly UIColor VenueCellAddressText = UIColor.FromRGB(153, 153, 153);

        public static readonly UIFont VenueShowCellTimeFont = UIFont.FromName(BrandonRegular, 12);
        public static readonly UIColor VenueShowCellTimeText = UIColor.Black;
        public static readonly UIFont VenueShowCellFont = UIFont.FromName(BrandonRegular, 14);
        public static readonly UIColor VenueShowCellText = UIColor.Black;
        public const int VenueShowTextLineHeight = 20;

        public static readonly UIColor MapPinText = UIColor.White;
        public static readonly UIFont MapPinFont = UIFont.FromName(BrandonBold, 14);
        public static readonly PointF MapPinOffset = new PointF(-8, -4);
        public static readonly PointF MapPinTextOffset = new PointF(5, 5);

        public static readonly UIColor HyperlinkText = UIColor.FromRGB(74, 197, 144);

        public static readonly UIColor Aqua = UIColor.FromRGB(0, 128, 255);
        public static readonly UIColor Mercury = UIColor.FromRGB(230, 230, 230);
        public static readonly UIColor MercuryLight = UIColor.FromRGB(240, 240, 240);
        public static readonly UIColor BlackLight = UIColor.FromRGB(49, 49, 49);

        public static void Apply()
        {
            UINavigationBar.Appearance.SetBackgroundImage(
                NavBarBackgroundImage,
                UIBarMetrics.Default);
            // TODO: bottom line has different color, bug?
            UINavigationBar.Appearance.SetBackgroundImage(
                NavBarLandscapeBackgroundImage,
                UIBarMetrics.LandscapePhone);
            UINavigationBar.Appearance.ShadowImage = ShadowImage;

            UINavigationBar.Appearance.SetTitleTextAttributes(new UITextAttributes { 
                Font = Theme.NavBarFont,
                TextColor = Theme.NavBarText,
                TextShadowColor = UIColor.Clear,
                TextShadowOffset = new UIOffset(0, 0)
            });
            UINavigationBar.Appearance.SetTitleVerticalPositionAdjustment(2, UIBarMetrics.Default);
            
            UIToolbar.Appearance.SetBackgroundImage(
                NavBarBackgroundImage,
                UIToolbarPosition.Any,
                UIBarMetrics.Default);
            UIToolbar.Appearance.SetBackgroundImage(
                NavBarLandscapeBackgroundImage,
                UIToolbarPosition.Any,
                UIBarMetrics.LandscapePhone);

            var cellAp = UICollectionViewCell.AppearanceWhenContainedIn(typeof(HomeView));
            cellAp.BackgroundColor = Theme.CellBackground;

            var labelAp = UILabel.AppearanceWhenContainedIn(typeof(OrgCell));
            labelAp.Font = Theme.OrgText;
            labelAp.TextColor = Theme.CellText;
            labelAp.HighlightedTextColor = Theme.CellTextHighlight;

            labelAp = UILabel.AppearanceWhenContainedIn(typeof(GroupHeaderCell));
            labelAp.Font = Theme.GroupHeaderText;
            labelAp.TextColor = Theme.CellText;

            labelAp = UILabel.AppearanceWhenContainedIn(typeof(EntityCell));
            labelAp.Font = Theme.EntityDescrFont;
            labelAp.TextColor = Theme.EntityDescrText;

            labelAp = UILabel.AppearanceWhenContainedIn(typeof(OrgEventHeaderView));
            labelAp.Font = Theme.OrgEventHeaderFont;
            labelAp.TextColor = Theme.OrgEventHeaderText;

            //var switchAp = UISwitch.AppearanceWhenContainedIn(typeof(OrgEventHeaderView));
            //switchAp.TintColor = NavBarBackground;
        }
    }
}