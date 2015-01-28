using System;
using CoreGraphics;
using UIKit;
using Foundation;

namespace SmartWalk.Client.iOS.Utils
{
    public static class ScreenUtil
    {
        public static readonly float HairLine = 1 / (2 * (float)UIScreen.MainScreen.Scale);

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

        public static float GetGoldenRatio(nfloat frameHeight)
        {
            var result = (float)Math.Ceiling(2 * (frameHeight / 5));
            return result;
        }

        public static float GetProportionalHeight(CGSize defaultSize, nfloat frameWidth)
        {
            var result = frameWidth * defaultSize.Height / defaultSize.Width;
            return (float)result;
        }

        public static float CalculateTextWidth(nfloat frameHeight, string text, UIFont font)
        {
            return (float)CalculateTextSize(new CGSize(float.MaxValue, frameHeight), text, font).Width;
        }

        public static float CalculateTextHeight(nfloat frameWidth, string text, UIFont font)
        {
            return (float)CalculateTextSize(new CGSize(frameWidth, float.MaxValue), text, font).Height;
        }

        public static CGSize CalculateTextSize(CGSize frame, string text, UIFont font)
        {
            if (!string.IsNullOrEmpty(text))
            {
                CGRect textSize;

                using (var ns = new NSString(text))
                {
                    textSize = ns.GetBoundingRect(
                        frame,
                        NSStringDrawingOptions.UsesLineFragmentOrigin |
                        NSStringDrawingOptions.UsesFontLeading,
                        new UIStringAttributes { Font = font },
                        null);
                }

                return new CGSize(
                    (float)Math.Ceiling(textSize.Width), 
                    (float)Math.Ceiling(textSize.Height));
            }

            return CGSize.Empty;
        }
    }
}