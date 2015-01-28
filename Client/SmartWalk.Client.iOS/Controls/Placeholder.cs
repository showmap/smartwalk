using System;
using UIKit;
using Foundation;

namespace SmartWalk.Client.iOS.Controls
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