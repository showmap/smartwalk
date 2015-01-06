using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Shared.Utils;
using SmartWalk.Client.iOS.Resources;

namespace SmartWalk.Client.iOS.Controls
{
    [Register("Circle")]
    public class Circle : UIView
    {
        private UIColor _color = Theme.CellSeparator;
        private float _lineWidth = 1;

        public Circle(IntPtr p) : base(p)
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
                    Draw(Bounds);
                }
            }
        }

        public float LineWidth
        {
            get
            {
                return _lineWidth;
            }
            set
            {
                if (_lineWidth.EqualsF(value))
                {
                    _lineWidth = value;
                    Draw(Bounds);
                }
            }
        }

        public override void Draw(RectangleF rect)
        {
            base.Draw(rect);

            var context = UIGraphics.GetCurrentContext();
            if (context != null)
            {
                context.SetStrokeColor(Color.CGColor);
                context.SetLineWidth(LineWidth);
                context.StrokeEllipseInRect(
                    new RectangleF(
                        LineWidth, 
                        LineWidth, 
                        Bounds.Width - 2 * LineWidth, 
                        Bounds.Height - 2 * LineWidth));
            }
        }
    }
}