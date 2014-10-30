using System;
using System.Drawing;
using MonoTouch.UIKit;

namespace SmartWalk.Client.iOS.Utils
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

        public static float GetGoldenRatio(float frameHeight)
        {
            var result = (float)Math.Ceiling(2 * (frameHeight / 5));
            return result;
        }

        public static float GetProportionalHeight(
            SizeF defaultSize,
            SizeF containerSize)
        {
            var containerWidth = Math.Min(containerSize.Width, containerSize.Height);
            var result = containerWidth * defaultSize.Height / defaultSize.Width;
            return result;
        }
    }
}