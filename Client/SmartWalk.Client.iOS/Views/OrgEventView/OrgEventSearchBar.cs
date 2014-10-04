using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Client.iOS.Utils;
using SmartWalk.Client.iOS.Resources;

namespace SmartWalk.Client.iOS.Views.OrgEventView
{
    // HACK: To handle iOS7 search bar size weird behavior
    [Register("OrgEventSearchBar")]
    public class OrgEventSearchBar : UISearchBar
    {
        private bool _isListOptionsVisible;

        public OrgEventSearchBar(IntPtr handle) : base(handle)
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

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            ConsoleUtil.LogDisposed(this);
        }

        private RectangleF GetFixedFrame(RectangleF frame)
        {
            if (Superview != null && 
                SearchBarStyle != UISearchBarStyle.Prominent)
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

    public static class OrgEventSearchBarExtensions
    {
        public static void SetPassiveStyle(this UISearchBar searchBar)
        {
            searchBar.BarStyle = UIBarStyle.Default;
            searchBar.TintColor = Theme.SearchControl;
            searchBar.BarTintColor = null;
            searchBar.SearchBarStyle = UISearchBarStyle.Minimal;
        }

        public static void SetActiveStyle(this UISearchBar searchBar)
        {
            searchBar.BarStyle = UIBarStyle.Default;
            searchBar.TintColor = Theme.SearchControl;
            searchBar.BarTintColor = Theme.NavBarBackgroundiOS7;
            searchBar.SearchBarStyle = UISearchBarStyle.Prominent;
        }
    }
}