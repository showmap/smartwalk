using System;
using CoreGraphics;
using Foundation;
using UIKit;
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

        public override CGRect Frame
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

        private CGRect GetFixedFrame(CGRect frame)
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
            searchBar.TintColor = ThemeColors.BorderLight;
            searchBar.BarTintColor = null;
            searchBar.SearchBarStyle = UISearchBarStyle.Minimal;
        }

        public static void SetActiveStyle(this UISearchBar searchBar)
        {
            searchBar.BarStyle = UIBarStyle.Default;
            searchBar.TintColor = ThemeColors.BorderLight;
            searchBar.BarTintColor = ThemeColors.HeaderBackground;
            searchBar.SearchBarStyle = UISearchBarStyle.Prominent;
        }
    }
}