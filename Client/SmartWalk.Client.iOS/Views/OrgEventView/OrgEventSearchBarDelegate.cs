using System;
using SmartWalk.Client.Core.ViewModels;
using SmartWalk.Client.iOS.Resources;
using SmartWalk.Client.iOS.Utils;
using UIKit;

namespace SmartWalk.Client.iOS.Views.OrgEventView
{
    public class OrgEventSearchBarDelegate : UISearchBarDelegate
    {
        private readonly OrgEventViewModel _viewModel;

        public event EventHandler SearchBegan;
        public event EventHandler SearchEnded;

        public OrgEventSearchBarDelegate(OrgEventViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        public void BeginSearch(UISearchBar searchBar)
        {
            NavBarManager.Instance.SetHidden(true, false);

            searchBar.SetActiveStyle();

            if (_viewModel.BeginSearchCommand.CanExecute(null))
            {
                _viewModel.BeginSearchCommand.Execute(null);
            }

            if (SearchBegan != null)
            {
                SearchBegan(this, EventArgs.Empty);
            }
        }

        public void EndSearch(UISearchBar searchBar)
        {
            searchBar.ResignFirstResponder();
            searchBar.Text = null;
            searchBar.SetPassiveStyle();

            NavBarManager.Instance.SetHidden(false, true);

            if (_viewModel.EndSearchCommand.CanExecute(null))
            {
                _viewModel.EndSearchCommand.Execute(null);
            }

            if (SearchEnded != null)
            {
                SearchEnded(this, EventArgs.Empty);
            }
        }

        public override void TextChanged(UISearchBar searchBar, string searchText)
        {
            if (_viewModel.SearchCommand.CanExecute(searchText))
            {
                _viewModel.SearchCommand.Execute(searchText);
            }
        }

        public override void OnEditingStarted(UISearchBar searchBar)
        {
            BeginSearch(searchBar);
        }

        public override void CancelButtonClicked(UISearchBar searchBar)
        {
            EndSearch(searchBar);
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
            searchBar.BackgroundColor = ThemeColors.PanelBackgroundAlpha;
            searchBar.ShowsCancelButton = false;
        }

        public static void SetActiveStyle(this UISearchBar searchBar)
        {
            searchBar.BackgroundColor = ThemeColors.PanelBackground;
            searchBar.ShowsCancelButton = true;
        }
    }
}