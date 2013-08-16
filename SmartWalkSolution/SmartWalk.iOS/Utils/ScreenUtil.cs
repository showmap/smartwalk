using MonoTouch.UIKit;

namespace SmartWalk.iOS.Utils
{
    public static class ScreenUtil
    {
        public static bool IsVerticalOrientation
        {
            get
            {
                return GetIsVerticalOrientation(UIApplication.SharedApplication.StatusBarOrientation);
            }
        }

        public static bool GetIsVerticalOrientation(UIInterfaceOrientation orientation)
        {
            return orientation == UIInterfaceOrientation.Portrait || 
                orientation == UIInterfaceOrientation.PortraitUpsideDown;
        }
    }
}