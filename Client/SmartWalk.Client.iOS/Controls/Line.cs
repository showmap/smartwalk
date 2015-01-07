using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Client.iOS.Resources;

namespace SmartWalk.Client.iOS.Controls
{
    [Register("Line")]
    public class Line : UIView
    {
        private UIColor _color = ThemeColors.BorderLight;
        private bool _isLineOnTop;

        public Line(IntPtr p) : base(p)
        {
            ContentMode = UIViewContentMode.Redraw;
        }

        public UIColor Color
        {
            get
            {
                return _color;
            }
            set
            {
                if (_color != value)
                {
                    _color = value;
                    SetNeedsDisplay();
                }
            }
        }

        public bool IsLineOnTop
        {
            get
            {
                return _isLineOnTop;
            }
            set
            {
                if (_isLineOnTop != value)
                {
                    _isLineOnTop = value;
                    SetNeedsDisplay();
                }
            }
        }

        public override void Draw(RectangleF rect)
        {
            base.Draw(rect);

            var context = UIGraphics.GetCurrentContext();
            if (context != null)
            {
                var lineWidth = 1 / UIScreen.MainScreen.Scale;

                context.SetFillColor(Color.CGColor);
                context.FillRect(new RectangleF(0, IsLineOnTop ? 0 : 1 - lineWidth, rect.Width, lineWidth));
            }
        }
    }
}