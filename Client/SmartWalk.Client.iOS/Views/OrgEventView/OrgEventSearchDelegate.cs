using CoreGraphics;
using UIKit;
using SmartWalk.Client.Core.ViewModels;
using SmartWalk.Client.iOS.Utils;

namespace SmartWalk.Client.iOS.Views.OrgEventView
{
    public class OrgEventSearchDelegate : UISearchDisplayDelegate
    {
        private readonly OrgEventHeaderView _headerView;
        private readonly OrgEventViewModel _viewModel;

        private UITableView _tableView;
        private OrgEventViewMode _previousMode;
        private bool _previousIsListOptVisible;
        private CGPoint _previousOffset;
        private UIEdgeInsets _previousContentInset;

        public OrgEventSearchDelegate(UITableView tableView, OrgEventViewModel viewModel)
        {
            _tableView = tableView;
            _headerView = (OrgEventHeaderView)tableView.TableHeaderView;
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
            NavBarManager.Instance.SetHidden(true, false);

            _previousOffset = _tableView.ContentOffset;
            _previousContentInset = _tableView.ContentInset;
            _tableView.ContentInset = UIEdgeInsets.Zero;

            if (_viewModel.SwitchModeCommand.CanExecute(OrgEventViewMode.List))
            {
                _previousMode = _viewModel.Mode;
                _viewModel.SwitchModeCommand.Execute(OrgEventViewMode.List);
            }

            _previousIsListOptVisible = _headerView.IsListOptionsVisible;
            _headerView.IsListOptionsVisible = false;
            controller.SearchBar.SetActiveStyle();
        }

        public override void WillEndSearch(UISearchDisplayController controller)
        {
        }

        public override void DidEndSearch(UISearchDisplayController controller)
        {
            _tableView.ContentInset = _previousContentInset;
            _tableView.ContentOffset = _previousOffset;

            _headerView.IsListOptionsVisible = _previousIsListOptVisible;
            controller.SearchBar.SetPassiveStyle();

            if (_viewModel.SwitchModeCommand.CanExecute(_previousMode))
            {
                _viewModel.SwitchModeCommand.Execute(_previousMode);
            }

            NavBarManager.Instance.SetHidden(false, true);
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