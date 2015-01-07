using MonoTouch.UIKit;

namespace SmartWalk.Client.iOS.Resources
{
    public static class ThemeColors
    {
        private static readonly UIColor Light = UIColor.White;
        private static readonly UIColor Twilight = UIColor.FromRGB(153, 153, 153);
        private static readonly UIColor Dark = UIColor.FromRGB(8, 8, 8);

        public static readonly UIColor ContentLightBackground = Light;
        public static readonly UIColor ContentLightText = Dark;
        public static readonly UIColor ContentLightTextPassive = Twilight;
        public static readonly UIColor ContentLightHighlight = UIColor.FromRGB(190, 190, 190);

        public static readonly UIColor ContentDarkBackground = Dark;
        public static readonly UIColor ContentDarkText = Light;

        public static readonly UIColor BorderLight = UIColor.FromRGB(230, 231, 233);
        public static readonly UIColor BorderDark = Twilight;

        public static readonly UIColor HeaderBackground = UIColor.FromRGB(100, 31, 174);
        public static readonly UIColor PanelBackground = UIColor.FromRGB(251, 251, 251);
        public static readonly UIColor SubPanelBackground = UIColor.FromRGB(255, 248, 218);

        public static readonly UIColor Action = UIColor.FromRGB(143, 44, 250);
        public static readonly UIColor Metadata = UIColor.FromRGB(88, 226, 194);
        public static readonly UIColor Subtitle = UIColor.FromRGB(255, 215, 51);


        public static readonly UIColor ContentLightBackgroundAlpha = ContentLightBackground.ColorWithAlpha(0.96f);
        public static readonly UIColor PanelBackgroundAlpha = PanelBackground.ColorWithAlpha(0.96f);
    }
}