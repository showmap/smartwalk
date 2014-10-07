using MonoTouch.UIKit;

namespace SmartWalk.Client.iOS.Views.OrgEventView
{
    public class OrgEventSearchDisplayController : UISearchDisplayController
    {
        public OrgEventSearchDisplayController(
            UISearchBar searchBar, 
            UIViewController viewController) 
            : base(searchBar, viewController) {}

        public override void SetActive(bool visible, bool animated)
        {
            base.SetActive(visible, animated);

            ((OrgEventView)SearchContentsController).SetNeedStatusBarUpdate(animated);
        }
    }
}