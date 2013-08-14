using MonoTouch.UIKit;
using System.Drawing;

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

        public static SizeF CurrentScreenSize
        {
            get
            {
                return IsVerticalOrientation 
                    ? UIScreen.MainScreen.Bounds.Size 
                        : new SizeF(
                            UIScreen.MainScreen.Bounds.Size.Height,
                            UIScreen.MainScreen.Bounds.Size.Width);
            }
        }
    }
}