using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace SmartWalk.Client.iOS.Views.OrgEventView
{
    // HACK: To handle iOS7 search bar size weird behavior
    [Register("CustomUISearchBar")]
    public class CustomUISearchBar : UISearchBar
    {
        private bool _isListOptionsVisible = true;

        public CustomUISearchBar(IntPtr Handle) : base(Handle)
        {
        }

        public bool IsListOptionsVisible
        {
            get
            {
                return _isListOptionsVisible;
            }
            set
            {
                if (_isListOptionsVisible != value)
                {
                    _isListOptionsVisible = value;
                    base.Frame = GetFixedFrame(Frame);
                }
            }
        }

        public override RectangleF Frame
        {
            set
            {
                base.Frame = GetFixedFrame(value);
            }
        }

        private RectangleF GetFixedFrame(RectangleF frame)
        {
            if (Superview != null)
            {
                frame.Width = Superview.Frame.Width - 
                    (IsListOptionsVisible 
                        ? OrgEventHeaderView.OptionsButtonWith
                        : 0);
                frame.Height = OrgEventHeaderView.DefaultHeight;
            }

            return frame;
        }
    }
}