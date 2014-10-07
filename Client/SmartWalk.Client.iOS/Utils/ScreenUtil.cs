using System;
using MonoTouch.UIKit;

namespace SmartWalk.Client.iOS.Utils
{
    public static class ScreenUtil
    {
        public const double Epsilon = 0.00001;

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

        public static float GetGoldenRatio(float frameHeight)
        {
            var result = (float)Math.Ceiling(2 * (frameHeight / 5));
            return result;
        }

        public static bool EqualsF(this float left, float right)
        {
            return Math.Abs(left - right) < Epsilon;
        }

        public static bool EqualsF(this double left, float right)
        {
            return EqualsF((float)left, right);
        }
    }
}