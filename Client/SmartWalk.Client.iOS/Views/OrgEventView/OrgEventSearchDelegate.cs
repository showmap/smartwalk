using System;
using SmartWalk.Client.Core.ViewModels;
using SmartWalk.Client.iOS.Utils;
using UIKit;

namespace SmartWalk.Client.iOS.Views.OrgEventView
{
    public class OrgEventSearchDelegate : UISearchDisplayDelegate
    {
        private readonly OrgEventViewModel _viewModel;

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
            NavBarManager.Instance.SetNativeHidden(false, false);

            controller.SearchBar.SetActiveStyle();

            if (BeginSearch != null)
            {
                BeginSearch(this, EventArgs.Empty);
            }

            if (_viewModel.BeginSearchCommand.CanExecute(null))
            {
                _viewModel.BeginSearchCommand.Execute(null);
            }
        }

        public override void DidEndSearch(UISearchDisplayController controller)
        {
            if (EndSearch != null)
            {
                EndSearch(this, EventArgs.Empty);
            }

            if (_viewModel.EndSearchCommand.CanExecute(null))
            {
                _viewModel.EndSearchCommand.Execute(null);
            }

            controller.SearchBar.SetPassiveStyle();

            NavBarManager.Instance.SetNativeHidden(true, false);
            NavBarManager.Instance.SetHidden(false, false);
            NavBarManager.Instance.RefreshContent(false);
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

    public static class OrgEventSearchBarExtensions
    {
        public static void SetPassiveStyle(this UISearchBar searchBar)
        {
            searchBar.SearchBarStyle = UISearchBarStyle.Minimal;
        }

        public static void SetActiveStyle(this UISearchBar searchBar)
        {
            searchBar.SearchBarStyle = UISearchBarStyle.Prominent;
        }
    }
}