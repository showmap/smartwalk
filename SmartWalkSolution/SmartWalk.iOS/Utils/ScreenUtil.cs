using MonoTouch.UIKit;

namespace SmartWalk.iOS.Utils
{
    public static class ScreenUtil
    {
        public static bool IsVerticalOrientation
        {
            get
            {
                return UIApplication.SharedApplication.StatusBarOrientation == UIInterfaceOrientation.Portrait || 
                    UIApplication.SharedApplication.StatusBarOrientation == UIInterfaceOrientation.PortraitUpsideDown;
            }
        }

        public static float CurrentScreenWidth
        {
            get
            {
                return IsVerticalOrientation 
                    ? UIScreen.MainScreen.Bounds.Width 
                        : UIScreen.MainScreen.Bounds.Height;
            }
        }
    }
}