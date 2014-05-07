using System.Drawing;
using Cirrious.MvvmCross.Binding.Touch.Views;
using MonoTouch.UIKit;

namespace SmartWalk.Client.iOS.Views.Common.Base
{
    public abstract class CustomNavBarTableViewSource : MvxTableViewSource
    {
        protected CustomNavBarTableViewSource(UITableView tableView) : base(tableView)
        {
        }

        public override void Scrolled(UIScrollView scrollView)
        {
            UIApplication.SharedApplication.SetStatusBarHidden(
                scrollView.ContentOffset != PointF.Empty, 
                UIStatusBarAnimation.Slide);
        }
    }
}