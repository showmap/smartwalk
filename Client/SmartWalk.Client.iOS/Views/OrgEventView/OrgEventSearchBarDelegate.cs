using System;
using System.Linq;
using Foundation;
using SmartWalk.Client.Core.ViewModels;
using SmartWalk.Client.iOS.Resources;
using SmartWalk.Client.iOS.Utils;
using SmartWalk.Shared.Utils;
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

            RemoveSearchButtonObserver(searchBar);
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

        public override void OnEditingStopped(UISearchBar searchBar)
        {
            // HACK: To keep Cancel button enabled on searchbar's focus lost
            if (searchBar.ShowsCancelButton)
            {
                AddSearchButtonObserver(searchBar);
            }
        }

        public override void CancelButtonClicked(UISearchBar searchBar)
        {
            EndSearch(searchBar);
        }

        public override void ObserveValue(NSString keyPath, NSObject ofObject, 
            NSDictionary change, IntPtr context)
        {
            var control = ofObject as UIControl;
            if (keyPath == control.GetPropertyName(c => c.Enabled).ToLower() && 
                control != null && !control.Enabled)
            {
                control.Enabled = true;
                RemoveSearchButtonObserver(control.ParentOfType<UISearchBar>());
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            ConsoleUtil.LogDisposed(this);
        }

        private void AddSearchButtonObserver(UISearchBar searchBar)
        {
            var controls = searchBar
                .GetAllSubViews()
                .OfType<UIControl>();
            foreach (var control in controls)
            {
                control.AddObserver(this, 
                    control.GetPropertyName(c => c.Enabled).ToLower(), 
                    NSKeyValueObservingOptions.New, IntPtr.Zero);
            }
        }

        private void RemoveSearchButtonObserver(UISearchBar searchBar)
        {
            var controls = searchBar
                .GetAllSubViews()
                .OfType<UIControl>();
            foreach (var control in controls)
            {
                try
                {
                    control.RemoveObserver(this, 
                        control.GetPropertyName(c => c.Enabled).ToLower());
                }
                catch (MonoTouchException)
                {
                }
            }
        }
    }

    public static class OrgEventSearchBarExtensions
    {
        public static void SetPassiveStyle(this UISearchBar searchBar)
        {
            searchBar.BackgroundColor = ThemeColors.PanelBackgroundAlpha;
            searchBar.SetShowsCancelButton(false, true);
        }

        public static void SetActiveStyle(this UISearchBar searchBar)
        {
            searchBar.BackgroundColor = ThemeColors.PanelBackground;
            searchBar.SetShowsCancelButton(true, true);
        }
    }
}