using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace SmartWalk.iOS.Controls
{
    [Register("Placeholder")]
    public class Placeholder : UIView
    {
        private UIView _content;

        public Placeholder(IntPtr handle) : base(handle)
        {
        }

        public UIView Content
        {
            get
            {
                return _content;
            }
            set
            {
                if (_content != null)
                {
                    _content.RemoveFromSuperview();
                }

                _content = value;

                if (_content != null)
                {
                    Add(_content);
                }

                SetNeedsLayout();
            }
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            if (Content != null)
            {
                Content.Frame = Bounds;
            }
        }
    }
}