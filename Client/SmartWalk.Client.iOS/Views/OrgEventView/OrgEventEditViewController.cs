using MonoTouch.EventKitUI;
using MonoTouch.UIKit;

namespace SmartWalk.Client.iOS.Views.OrgEventView
{
    public class OrgEventEditViewController : EKEventEditViewController
    {
        public override UIStatusBarStyle PreferredStatusBarStyle()
        {
            return UIStatusBarStyle.LightContent;
        }
    }
}