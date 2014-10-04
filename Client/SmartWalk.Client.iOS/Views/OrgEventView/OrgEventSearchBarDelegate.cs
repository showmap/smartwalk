using MonoTouch.UIKit;

namespace SmartWalk.Client.iOS.Views.OrgEventView
{
    public class OrgEventSearchBarDelegate : UISearchBarDelegate
    {
        public override bool ShouldBeginEditing(UISearchBar searchBar)
        {
            searchBar.SetActiveStyle();
            return true;
        }

        public override bool ShouldEndEditing(UISearchBar searchBar)
        {
            searchBar.SetPassiveStyle();
            return true;
        }
    }
}