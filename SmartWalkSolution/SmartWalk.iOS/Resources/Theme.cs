using MonoTouch.UIKit;
using SmartWalk.iOS.Views.Common;
using SmartWalk.iOS.Views.Common.EntityCell;
using SmartWalk.iOS.Views.HomeView;

namespace SmartWalk.iOS.Resources
{
    public class Theme
    {
        public static readonly UIColor NavBarBackground = UIColor.FromRGB(51, 51, 51);
        public static readonly UIColor NavBarText = UIColor.White;
        public static readonly UIFont NavBarFont = UIFont.FromName("BrandonGrotesque-Black", 14);

        public static readonly UIColor CellBackground = UIColor.White;
        public static readonly UIColor CellHighlight = UIColor.FromRGB(255, 216, 0);
        public static readonly UIColor CellText = UIColor.Black;
        public static readonly UIColor CellTextHighlight = UIColor.White;

        public static readonly UIFont OrgText = UIFont.FromName("BrandonGrotesque-Regular", 24);
        public const int OrgTextLineSpacing = -9;
        public static readonly UIFont GroupHeaderText = UIFont.FromName("BrandonGrotesque-Black", 14);

        public static readonly UIColor OrgEventDayText = UIColor.White;
        public static readonly UIFont OrgEventWeekDayFont = UIFont.FromName("BrandonGrotesque-Black", 10);
        public static readonly UIFont OrgEventDayFont = UIFont.FromName("BrandonGrotesque-Regular", 18);
        public static readonly UIColor OrgEventDateText = UIColor.FromRGB(102, 102, 102);
        public static readonly UIFont OrgEventDateFont = UIFont.FromName("BrandonGrotesque-Regular", 18);
        public static readonly UIColor OrgEventHintText = UIColor.FromRGB(187, 187, 187);
        public static readonly UIFont OrgEventHintFont = UIFont.FromName("BrandonGrotesque-Regular", 18);
        public static readonly UIColor OrgEventActive = CellHighlight;
        public static readonly UIColor OrgEventPassive = UIColor.FromRGB(187, 187, 187);

        public static readonly UIColor EntityDescrText = UIColor.FromRGB(102, 102, 102);
        public static readonly UIFont EntityDescrFont = UIFont.FromName("BrandonGrotesque-Regular", 16);
        public const int EntityDescrTextLineHeight = 24;

        public static readonly UIColor Aqua = UIColor.FromRGB(0, 128, 255);
        public static readonly UIColor Mercury = UIColor.FromRGB(230, 230, 230);
        public static readonly UIColor MercuryLight = UIColor.FromRGB(240, 240, 240);
        public static readonly UIColor BlackLight = UIColor.FromRGB(49, 49, 49);

        public static void Apply()
        {
            UINavigationBar.Appearance.SetBackgroundImage(
                UIImage.FromFile("Images/NavBarBackground.png"), 
                UIBarMetrics.Default);
            UINavigationBar.Appearance.SetBackgroundImage(
                UIImage.FromFile("Images/NavBarLandscapeBackground.png"), 
                UIBarMetrics.LandscapePhone); // TODO: bottom line has different color, bug?
            UINavigationBar.Appearance.ShadowImage = 
                UIImage.FromFile("Images/Shadow.png")
                    .CreateResizableImage(new UIEdgeInsets(0, 1, 0, 1));

            UINavigationBar.Appearance.SetTitleTextAttributes(new UITextAttributes { 
                Font = Theme.NavBarFont,
                TextColor = Theme.NavBarText,
                TextShadowColor = UIColor.Clear,
                TextShadowOffset = new UIOffset(0, 0)
            });
            UINavigationBar.Appearance.SetTitleVerticalPositionAdjustment(2, UIBarMetrics.Default);

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
        }
    }
}