using System;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using SmartWalk.Client.iOS.Resources;
using UIKit;

namespace SmartWalk.Client.iOS.Controls
{
    [Register("Gradient")]
    public class Gradient : UIView
    {
        private CAGradientLayer _bottomGradient;

        public Gradient(IntPtr p) : base(p)
        {
        }

        public override void Draw(CGRect rect)
        {
            base.Draw(rect);

            if (_bottomGradient == null)
            {
                _bottomGradient = new CAGradientLayer {
                    Frame = rect,
                    Colors = new [] { 
                        ThemeColors.ContentDarkBackground.ColorWithAlpha(0.2f).CGColor, 
                        ThemeColors.ContentDarkBackground.ColorWithAlpha(0.8f).CGColor 
                    },
                    Locations = new [] {
                        new NSNumber(0),
                        new NSNumber(1)
                    },
                    ShouldRasterize = true,
                    RasterizationScale = UIScreen.MainScreen.Scale
                };

                Layer.InsertSublayer(_bottomGradient, 0);
            }
            else
            {
                _bottomGradient.Frame = rect;
            }
        }
    }
}