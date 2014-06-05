using MonoTouch.UIKit;
using SmartWalk.Client.Core.ViewModels;
using SmartWalk.Client.iOS.Utils;

namespace SmartWalk.Client.iOS.Views.OrgEventView
{
    public class OrgEventSearchDelegate : UISearchDisplayDelegate
    {
        private readonly OrgEventHeaderView _headerView;
        private readonly OrgEventViewModel _viewModel;

        private OrgEventViewMode _previousMode;
        private bool _previousIsListOptVisible;

        public OrgEventSearchDelegate(
            OrgEventHeaderView headerView,
            OrgEventViewModel viewModel)
        {
            _headerView = headerView;
            _viewModel = viewModel;
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

        public override void WillBeginSearch(UISearchDisplayController controller)
        {
            if (_viewModel.SwitchModeCommand.CanExecute(OrgEventViewMode.List))
            {
                _previousMode = _viewModel.Mode;
                _viewModel.SwitchModeCommand.Execute(OrgEventViewMode.List);
            }

            _previousIsListOptVisible = _headerView.IsListOptionsVisible;
            _headerView.IsListOptionsVisible = false;
        }

        public override void WillEndSearch(UISearchDisplayController controller)
        {
            _headerView.IsListOptionsVisible = _previousIsListOptVisible;
        }

        public override void DidEndSearch(UISearchDisplayController controller)
        {
            if (_viewModel.SwitchModeCommand.CanExecute(_previousMode))
            {
                _viewModel.SwitchModeCommand.Execute(_previousMode);
            }
        }

        public override void WillShowSearchResults(UISearchDisplayController controller, UITableView tableView)
        {
            tableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;

            ((OrgEventTableSource)controller.SearchResultsSource).TableView = tableView;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            ConsoleUtil.LogDisposed(this);
        }
    }
}