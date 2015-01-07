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
        private UIColor _fillColor = UIColor.Clear;
        private UIColor _lineColor = ThemeColors.BorderLight;
        private float _lineWidth = 1;

        public Circle()
        {
            Initialize();
        }

        public Circle(IntPtr p) : base(p)
        {
            Initialize();
        }

        public UIColor FillColor
        {
            get
            {
                return _fillColor;
            }
            set
            {
                if (_fillColor != value)
                {
                    _fillColor = value;
                    SetNeedsDisplay();
                }
            }
        }

        public UIColor LineColor
        {
            get
            {
                return _lineColor;
            }
            set
            {
                if (_lineColor != value)
                {
                    _lineColor = value;
                    SetNeedsDisplay();
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
                if (!_lineWidth.EqualsF(value))
                {
                    _lineWidth = value;
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
                if (FillColor != UIColor.Clear)
                {
                    context.SetFillColor(FillColor.CGColor);
                    context.FillEllipseInRect(
                        new RectangleF(
                            LineWidth, 
                            LineWidth, 
                            Bounds.Width - 2 * LineWidth, 
                            Bounds.Height - 2 * LineWidth));
                }

                if (LineColor != UIColor.Clear && !LineWidth.EqualsF(0))
                {
                    context.SetLineWidth(LineWidth);
                    context.SetStrokeColor(LineColor.CGColor);
                    context.StrokeEllipseInRect(
                        new RectangleF(
                            LineWidth, 
                            LineWidth, 
                            Bounds.Width - 2 * LineWidth, 
                            Bounds.Height - 2 * LineWidth));
                }
            }
        }

        private void Initialize()
        {
            ContentMode = UIViewContentMode.Redraw;
            BackgroundColor = UIColor.Clear;
        }
    }
}