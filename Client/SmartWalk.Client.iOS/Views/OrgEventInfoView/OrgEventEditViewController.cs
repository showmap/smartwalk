using EventKitUI;
using UIKit;

namespace SmartWalk.Client.iOS.Views.OrgEventInfoView
{
    public class OrgEventEditViewController : EKEventEditViewController
    {
        public override UIStatusBarStyle PreferredStatusBarStyle()
        {
            return UIStatusBarStyle.LightContent;
        }
    }
}