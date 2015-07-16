using UIKit;
using SmartWalk.Client.iOS.Utils;

namespace SmartWalk.Client.iOS.Resources
{
    public static class ThemeIcons
    {
        public static readonly UIImage Back = Image("Icons/Back.png");
        public static readonly UIImage BackLandscape = Image("Icons/BackLandscape.png");
        public static readonly UIImage Forward = Image("Icons/Forward.png");
        public static readonly UIImage ForwardLandscape = Image("Icons/ForwardLandscape.png");
        public static readonly UIImage List = Image("Icons/List.png");
        public static readonly UIImage ListLandscape = Image("Icons/ListLandscape.png");
        public static readonly UIImage Map = Image("Icons/Map.png");
        public static readonly UIImage MapLandscape = Image("Icons/MapLandscape.png");
        public static readonly UIImage More = Image("Icons/More.png");
        public static readonly UIImage MoreLandscape = Image("Icons/MoreLandscape.png");
        public static readonly UIImage Refresh = Image("Icons/Refresh.png");
        public static readonly UIImage RefreshLandscape = Image("Icons/RefreshLandscape.png");
        public static readonly UIImage Close = Image("Icons/Close.png");
        public static readonly UIImage CloseLandscape = Image("Icons/CloseLandscape.png");

        public static readonly UIImage ListOptions = UIImage.FromFile("Icons/ListOptions.png"); // 3x ?
        public static readonly UIImage Fullscreen = Image("Icons/Fullscreen.png");
        public static readonly UIImage ExitFullscreen = Image("Icons/ExitFullscreen.png");
        public static readonly UIImage FullscreenLandscape = Image("Icons/FullscreenLandscape.png");
        public static readonly UIImage ExitFullscreenLandscape = Image("Icons/ExitFullscreenLandscape.png");
        public static readonly UIImage MapPin = UIImage.FromFile("Icons/MapPin.png");
        public static readonly UIImage MapPinSmall = UIImage.FromFile("Icons/MapPinSmall.png"); // 3x ?
        public static readonly UIImage Info = Image("Icons/Info.png");
        public static readonly UIImage DownLink = Image("Icons/DownLink.png");
        public static readonly UIImage Star = Image("Icons/Star.png");
        public static readonly UIImage StarOutline = Image("Icons/StarOutline.png");
            
        public static readonly UIImage ContactEmail = Image("Icons/ContactEmail.png");
        public static readonly UIImage ContactPhone = Image("Icons/ContactPhone.png");
        public static readonly UIImage ContactWeb = Image("Icons/ContactWeb.png");

        private static UIImage Image(string path)
        {
            var image = UIImage.FromFile(path);
            if (image != null)
            {
                var result = image.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
                return result;
            }

            ConsoleUtil.Trace("Failed to load '{0}' image.", path);
            return null;
        }
    }
}