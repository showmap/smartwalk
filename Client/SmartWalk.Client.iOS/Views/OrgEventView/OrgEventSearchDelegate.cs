using MonoTouch.UIKit;
using SmartWalk.Client.Core.ViewModels;
using SmartWalk.Client.iOS.Resources;
using SmartWalk.Client.iOS.Utils;

namespace SmartWalk.Client.iOS.Views.OrgEventView
{
    public class OrgEventSearchDelegate : UISearchDisplayDelegate
    {
        private readonly OrgEventViewModel _viewModel;

        public OrgEventSearchDelegate(OrgEventViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        public override void WillBeginSearch(UISearchDisplayController controller)
        {
            if (UIDevice.CurrentDevice.CheckSystemVersion(7, 0))
            {
                controller.SearchBar.BarTintColor = Theme.NavBarBackgroundiOS7;
            }
            else
            {
                controller.SearchBar.TintColor = Theme.NavBarBackground;
            }
        }

        public override void WillEndSearch(UISearchDisplayController controller)
        {
            if (UIDevice.CurrentDevice.CheckSystemVersion(7, 0))
            {
                controller.SearchBar.BarTintColor = null;

            }
            else
            {
                controller.SearchBar.TintColor = Theme.SearchControl;
            }
        }

        public override bool ShouldReloadForSearchString(
            UISearchDisplayController controller,
            string forSearchString)
        {
            if (_viewModel.SearchCommand.CanExecute(forSearchString))
            {
                _viewModel.SearchCommand.Execute(forSearchString);
            }

            return false;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            ConsoleUtil.LogDisposed(this);
        }
    }
}