using System;
using SmartWalk.Client.Core.ViewModels;
using SmartWalk.Client.iOS.Utils;
using UIKit;

namespace SmartWalk.Client.iOS.Views.OrgEventView
{
    public class OrgEventSearchDelegate : UISearchDisplayDelegate
    {
        private readonly OrgEventViewModel _viewModel;

        private OrgEventViewMode? _previousMode;

        public event EventHandler BeginSearch;
        public event EventHandler EndSearch;

        public OrgEventSearchDelegate(OrgEventViewModel viewModel)
        {
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

            if (BeginSearch != null)
            {
                BeginSearch(this, EventArgs.Empty);
            }

            if (_viewModel.Mode != OrgEventViewMode.List &&
                _viewModel.SwitchModeCommand.CanExecute(OrgEventViewMode.List))
            {
                _previousMode = _viewModel.Mode;
                _viewModel.SwitchModeCommand.Execute(OrgEventViewMode.List);
            }

            controller.SearchBar.SetActiveStyle();
        }

        public override void WillEndSearch(UISearchDisplayController controller)
        {
        }

        public override void DidEndSearch(UISearchDisplayController controller)
        {
            if (EndSearch != null)
            {
                EndSearch(this, EventArgs.Empty);
            }

            controller.SearchBar.SetPassiveStyle();

            if (_previousMode.HasValue &&
                _viewModel.SwitchModeCommand.CanExecute(_previousMode.Value))
            {
                _viewModel.SwitchModeCommand.Execute(_previousMode.Value);
                _previousMode = null;
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