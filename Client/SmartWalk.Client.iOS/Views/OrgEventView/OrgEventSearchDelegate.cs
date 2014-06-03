using MonoTouch.UIKit;
using SmartWalk.Client.Core.ViewModels;
using SmartWalk.Client.iOS.Resources;
using SmartWalk.Client.iOS.Utils;

namespace SmartWalk.Client.iOS.Views.OrgEventView
{
    public class OrgEventSearchDelegate : UISearchDisplayDelegate
    {
        private readonly OrgEventHeaderView _headerView;
        private readonly OrgEventViewModel _viewModel;

        private bool _previousIsListOptVisible;

        public OrgEventSearchDelegate(
            OrgEventHeaderView headerView, 
            OrgEventViewModel viewModel)
        {
            _headerView = headerView;
            _viewModel = viewModel;
        }

        public override void WillBeginSearch(UISearchDisplayController controller)
        {
            _previousIsListOptVisible = _headerView.IsListOptionsVisible;
            _headerView.IsListOptionsVisible = false;

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
            _headerView.IsListOptionsVisible = _previousIsListOptVisible;

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