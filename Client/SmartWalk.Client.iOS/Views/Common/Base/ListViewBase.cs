using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Client.Core.ViewModels.Interfaces;
using SmartWalk.Shared.Utils;
using SmartWalk.Client.iOS.Controls;
using SmartWalk.Client.iOS.Resources;
using SmartWalk.Client.iOS.Utils;

namespace SmartWalk.Client.iOS.Views.Common.Base
{
    public abstract class ListViewBase : CustomNavBarViewBase
    {
        private UIRefreshControl _refreshControl;
        private ListViewDecorator _listView;
        private UIView _progressView;
        private IListViewSource _listViewSource;

        protected string ViewTitle
        {
            get
            {
                var refreshableViewModel = ViewModel as IRefreshableViewModel;
                return refreshableViewModel != null ? refreshableViewModel.Title : null;
            }
        }

        private ListViewDecorator ListView 
        { 
            get
            {
                if (_listView == null)
                {
                    _listView = GetListView();
                }

                return _listView;
            }
        }

        private UIView ProgressViewContainer 
        { 
            get
            {
                if (_progressView == null)
                {
                    _progressView = GetProgressViewContainer();
                    var progress = ProgressView.Create();
                    _progressView.AddSubview(progress);
                }

                return _progressView;
            }
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var refreshableViewModel = ViewModel as IRefreshableViewModel;
            if (refreshableViewModel != null)
            {
                InitializeRefreshControl();
                refreshableViewModel.RefreshCompleted += OnViewModelRefreshCompleted;
            }

            UpdateViewTitle();
            UpdateViewState();

            InitializeConstraints();
            InitializeListView();
        }

        public override void WillMoveToParentViewController(UIViewController parent)
        {
            base.WillMoveToParentViewController(parent);

            if (parent == null)
            {
                var refreshableViewModel = ViewModel as IRefreshableViewModel;
                if (refreshableViewModel != null)
                {
                    refreshableViewModel.RefreshCompleted -= OnViewModelRefreshCompleted;
                }

                DisposeRefreshControl();
            }
        }

        protected abstract ListViewDecorator GetListView();

        protected abstract UIView GetProgressViewContainer();

        protected abstract NSLayoutConstraint GetProgressViewTopConstraint();

        protected virtual void InitializeListView()
        {
            OnBeforeSetListViewSource();

            _listViewSource = CreateListViewSource();
            ListView.Source = _listViewSource;
        }

        protected abstract IListViewSource CreateListViewSource();

        protected virtual void UpdateViewTitle()
        {
            NavigationItem.Title = ViewTitle ?? string.Empty;
        }

        protected virtual void OnBeforeSetListViewSource()
        {
        }

        protected virtual void OnViewModelRefreshed()
        {
            ListView.View.SetContentOffset(PointF.Empty, true);
        }

        protected virtual void OnLoadingViewStateUpdate()
        {
            ListView.View.Hidden = true;
        }

        protected virtual void OnLoadedViewStateUpdate()
        {
            UIView.Transition(
                ListView.View,
                ScrollUtil.ShowViewAnimationDuration,
                UIViewAnimationOptions.TransitionCrossDissolve,
                new NSAction(() => ListView.View.Hidden = false),
                null);
        }

        protected override void OnViewModelPropertyChanged(string propertyName)
        {
            base.OnViewModelPropertyChanged(propertyName);

            var progressViewModel = ViewModel as IProgressViewModel;
            if (progressViewModel != null &&
                propertyName == progressViewModel.GetPropertyName(p => p.IsLoading))
            {
                UpdateViewState();
            }

            var refreshableViewModel = ViewModel as IRefreshableViewModel;
            if (refreshableViewModel != null &&
                propertyName == refreshableViewModel.GetPropertyName(p => p.Title))
            {
                UpdateViewTitle();
            }
        }

        private void InitializeRefreshControl()
        {
            _refreshControl = new UIRefreshControl {
                TintColor = Theme.RefreshControl
            };

            // make sure that it's under all other views
            _refreshControl.Layer.ZPosition = -1;

            _refreshControl.ValueChanged += OnRefreshControlValueChanged;

            ListView.View.AddSubview(_refreshControl);
        }

        private void DisposeRefreshControl()
        {
            if (_refreshControl != null)
            {
                _refreshControl.ValueChanged -= OnRefreshControlValueChanged;
                _refreshControl.Dispose();
                _refreshControl = null;
            }
        }

        private void OnViewModelRefreshCompleted(object sender, EventArgs e)
        {
            _refreshControl.EndRefreshing();
            OnViewModelRefreshed();
        }

        private void OnRefreshControlValueChanged(object sender, EventArgs e)
        {
            var refreshableViewModel = ViewModel as IRefreshableViewModel;
            if (refreshableViewModel != null &&
                refreshableViewModel.RefreshCommand.CanExecute(null))
            {
                refreshableViewModel.RefreshCommand.Execute(null);
            }
        }

        private void InitializeConstraints()
        {
            if (UIDevice.CurrentDevice.CheckSystemVersion(7, 0))
            {
                var progressViewTopConstraint = GetProgressViewTopConstraint();

                if (ProgressViewContainer != null &&
                    progressViewTopConstraint != null)
                {
                    View.RemoveConstraint(progressViewTopConstraint);

                    var views = new NSDictionary("topGuide", TopLayoutGuide, "view", ProgressViewContainer);
                    var constraint = NSLayoutConstraint.FromVisualFormat("V:[topGuide]-0-[view]", 0, null, views);
                    View.AddConstraints(constraint);
                }
            }
        }

        private void UpdateViewState()
        {
            if (ListView.Source != null && ListView.Source.ItemsSource != null) return;

            var progressViewModel = (IProgressViewModel)ViewModel;
            if (progressViewModel.IsLoading)
            {
                ProgressViewContainer.Hidden = false;
                OnLoadingViewStateUpdate();
            }
            else
            {
                ProgressViewContainer.Hidden = true;
                OnLoadedViewStateUpdate();
            }
        }
    }
}