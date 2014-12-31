using MonoTouch.UIKit;

namespace SmartWalk.Client.iOS.Resources
{
    public static class ThemeIcons
    {
        public static readonly UIImage Back = UIImage.FromFile("Icons/Back.png");
        public static readonly UIImage BackLandscape = UIImage.FromFile("Icons/BackLandscape.png");
        public static readonly UIImage Forward = UIImage.FromFile("Icons/Forward.png")
            .ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
        public static readonly UIImage ForwardLandscape = UIImage.FromFile("Icons/ForwardLandscape.png")
            .ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
        public static readonly UIImage List = UIImage.FromFile("Icons/List.png");
        public static readonly UIImage ListLandscape = UIImage.FromFile("Icons/ListLandscape.png"); // Pixelated
        public static readonly UIImage Map = UIImage.FromFile("Icons/Map.png");
        public static readonly UIImage MapLandscape = UIImage.FromFile("Icons/MapLandscape.png");
        public static readonly UIImage More = UIImage.FromFile("Icons/More.png");
        public static readonly UIImage MoreLandscape = UIImage.FromFile("Icons/MoreLandscape.png"); // Missing
        public static readonly UIImage Refresh = UIImage.FromFile("Icons/Refresh.png");
        public static readonly UIImage RefreshLandscape = UIImage.FromFile("Icons/RefreshLandscape.png");
        public static readonly UIImage Close = UIImage.FromFile("Icons/Close.png");
        public static readonly UIImage CloseLandscape = UIImage.FromFile("Icons/CloseLandscape.png");

        public static readonly UIImage ListOptions = UIImage.FromFile("Icons/ListOptions.png"); // 3x ?, maybe violet?
        public static readonly UIImage Fullscreen = UIImage.FromFile("Icons/Fullscreen.png");
        public static readonly UIImage ExitFullscreen = UIImage.FromFile("Icons/ExitFullscreen.png");
        public static readonly UIImage FullscreenLandscape = UIImage.FromFile("Icons/FullscreenLandscape.png"); // Missing
        public static readonly UIImage ExitFullscreenLandscape = UIImage.FromFile("Icons/ExitFullscreenLandscape.png"); // Missing
        public static readonly UIImage MapPin = UIImage.FromFile("Icons/MapPin.png"); // 3x ?
        public static readonly UIImage MapPinSmall = UIImage.FromFile("Icons/MapPinSmall.png"); // 3x ?
        public static readonly UIImage Info = UIImage.FromFile("Icons/Info.png");
        public static readonly UIImage DownLink = UIImage.FromFile("Icons/DownLink.png")
            .ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);

        // Too big. Make just white? Background circle programmatically
        public static readonly UIImage ContactEmail = UIImage.FromFile("Icons/ContactEmail.png");
        public static readonly UIImage ContactPhone = UIImage.FromFile("Icons/ContactPhone.png"); // Pixelated
        public static readonly UIImage ContactWeb = UIImage.FromFile("Icons/ContactWeb.png");
    }
}