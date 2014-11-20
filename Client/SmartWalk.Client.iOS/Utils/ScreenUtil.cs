using System;
using System.Drawing;
using MonoTouch.UIKit;
using MonoTouch.Foundation;

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

        public static float GetProportionalHeight(SizeF defaultSize, float frameWidth)
        {
            var result = frameWidth * defaultSize.Height / defaultSize.Width;
            return result;
        }

        public static float CalculateTextHeight(float frameWidth, string text, UIFont font)
        {
            if (!string.IsNullOrEmpty(text))
            {
                var frameSize = new SizeF(frameWidth, float.MaxValue);

                RectangleF textSize;

                using (var ns = new NSString(text))
                {
                    textSize = ns.GetBoundingRect(
                        frameSize,
                        NSStringDrawingOptions.UsesLineFragmentOrigin |
                        NSStringDrawingOptions.UsesFontLeading,
                        new UIStringAttributes { Font = font },
                        null);
                }

                return (float)Math.Ceiling(textSize.Height);
            }

            return 0;
        }
    }
}