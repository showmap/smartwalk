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
        public CustomUISearchBar(IntPtr Handle) : base(Handle)
        {
        }

        public override RectangleF Frame
        {
            set
            {
                if (Superview != null)
                {
                    value.Width = Superview.Frame.Width - OrgEventHeaderView.OptionsButtonWith;
                    value.Height = OrgEventHeaderView.DefaultHeight;
                }

                base.Frame = value;
            }
        }
    }
}