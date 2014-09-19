using System;
using System.Drawing;
using Cirrious.CrossCore.Core;
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
        private UIView _progressViewContainer;
        private ProgressView _progressView;
        private IListViewSource _listViewSource;
        private UITapGestureRecognizer _viewTapGesture;

        protected string ViewTitle
        {
            get
            {
                var refreshableViewModel = ViewModel as IRefreshableViewModel;
                return refreshableViewModel != null ? refreshableViewModel.Title : null;
            }
        }

        protected bool IsLoading
        {
            get { return ViewModel is IProgressViewModel && ((IProgressViewModel)ViewModel).IsLoading; }
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

        private bool HasListData
        {
            get
            {
                return ListView.Source != null && ListView.Source.ItemsSource != null;
            }
        }
            
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var refreshableViewModel = ViewModel as IRefreshableViewModel;
            if (refreshableViewModel != null)
            {
                InitializeRefreshControl();
                refreshableViewModel.RefreshCompleted += OnRefreshCompleted;
            }

            InitializeConstraints();
            InitializeListView();
            InitializeProgressView();
            InitializeGesture();

            UpdateViewTitle();
            UpdateViewLoadingState();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            UpdateStatusBarLoadingState(animated);
        }

        public override void DidMoveToParentViewController(UIViewController parent)
        {
            base.DidMoveToParentViewController(parent);

            if (parent == null)
            {
                var refreshableViewModel = ViewModel as IRefreshableViewModel;
                if (refreshableViewModel != null)
                {
                    refreshableViewModel.RefreshCompleted -= OnRefreshCompleted;
                }

                DisposeRefreshControl();
                DisposeGesture();
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

            // hiding list view initially, to show content errors and loading indicators
            // TODO: To research, maybe we should keep it visible to allow Pull-To-Refresh
            // although the cases where search bar is present should be handled separately
            ListView.View.SetHidden(true, false);
        }

        protected abstract IListViewSource CreateListViewSource();

        protected virtual void UpdateStatusBarLoadingState(bool animated)
        {
            if (IsLoading)
            {
                SetStatusBarHidden(false, animated);
            }
            else
            {
                SetStatusBarHidden(true, animated);
            }
        }

        protected override void SetStatusBarHidden(bool hidden, bool animated)
        {
            // making sure that view is still visible on screen after loading finished
            if (IsActive)
            {
                hidden = !IsLoading && hidden;

                base.SetStatusBarHidden(hidden, animated);
            }
        }

        protected virtual void UpdateViewTitle()
        {
            NavigationItem.Title = ViewTitle ?? string.Empty;
        }

        protected virtual void OnBeforeSetListViewSource()
        {
        }

        protected virtual void OnViewModelRefreshed(bool hasData)
        {
            if (hasData)
            {
                ScrollViewToTop();
            }
            else
            {
                ListView.View.SetHidden(true, true);
            }
        }

        protected virtual void ScrollViewToTop()
        {
            ListView.View.SetContentOffset(PointF.Empty, true);
        }

        protected virtual void OnLoadingViewStateUpdate()
        {
            if (!HasListData)
            {
                ListView.View.SetHidden(true, false);
            }
        }

        protected virtual void OnLoadedViewStateUpdate()
        {
            if (HasListData)
            {
                ListView.View.SetHidden(false, true);
            }
        }

        protected override void OnViewModelPropertyChanged(string propertyName)
        {
            base.OnViewModelPropertyChanged(propertyName);

            var progressViewModel = ViewModel as IProgressViewModel;
            if (progressViewModel != null &&
                propertyName == progressViewModel.GetPropertyName(p => p.IsLoading))
            {
                UpdateViewLoadingState();
                UpdateStatusBarLoadingState(true);
            }

            var refreshableViewModel = ViewModel as IRefreshableViewModel;
            if (refreshableViewModel != null &&
                propertyName == refreshableViewModel.GetPropertyName(p => p.Title))
            {
                UpdateViewTitle();
            }
        }

        private void InitializeProgressView()
        {
            _progressViewContainer = GetProgressViewContainer();
            _progressView = ProgressView.Create();
            _progressViewContainer.AddSubview(_progressView);
        }

        private void InitializeGesture()
        {
            _viewTapGesture = new UITapGestureRecognizer(() => 
                ListView.View.SetContentOffset(PointF.Empty, true))
            {
                Delegate = new ListViewTapGestureDelegate()
            };

            View.AddGestureRecognizer(_viewTapGesture);
        }

        private void DisposeGesture()
        {
            if (_viewTapGesture != null)
            {
                View.RemoveGestureRecognizer(_viewTapGesture);
                _viewTapGesture.Dispose();
                _viewTapGesture = null;
            }
        }

        private void InitializeRefreshControl()
        {
            _refreshControl = new UIRefreshControl {
                TintColor = Theme.RefreshControl
            };

            // make sure that it's behind all other views
            _refreshControl.Layer.ZPosition = -100;

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

        private void OnRefreshCompleted(object sender, MvxValueEventArgs<bool> e)
        {
            if (_refreshControl.Refreshing)
            {
                _refreshControl.EndRefreshing();
            }

            UpdateViewDataState(e.Value);
            OnViewModelRefreshed(e.Value);
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

                if (_progressViewContainer != null &&
                    progressViewTopConstraint != null)
                {
                    View.RemoveConstraint(progressViewTopConstraint);

                    var views = new NSDictionary(
                        "topGuide", 
                        TopLayoutGuide, 
                        "view", 
                        _progressViewContainer);
                    var constraint = 
                        NSLayoutConstraint.FromVisualFormat("V:[topGuide]-0-[view]", 0, null, views);
                    View.AddConstraints(constraint);
                }
            }
        }

        private void UpdateViewLoadingState()
        {
            if (IsLoading)
            {
                if (!HasListData)
                {
                    _progressView.IsDataUnavailable = false;
                    _progressView.IsLoading = true;
                }

                OnLoadingViewStateUpdate();
            }
            else
            {
                UpdateViewDataState(HasListData);
                _progressView.IsLoading = false;
                OnLoadedViewStateUpdate();
            }
        }

        private void UpdateViewDataState(bool hasListData)
        {
            _progressView.IsDataUnavailable = !hasListData;
        }
    }

    public class ListViewTapGestureDelegate : UIGestureRecognizerDelegate
    {
        public override bool ShouldReceiveTouch(UIGestureRecognizer recognizer, UITouch touch)
        {
            return UIApplication.SharedApplication.StatusBarHidden &&
                touch.LocationInView(recognizer.View).Y < UIConstants.StatusBarHeight;
        }

        public override bool ShouldBeRequiredToFailBy(
            UIGestureRecognizer gestureRecognizer, 
            UIGestureRecognizer otherGestureRecognizer)
        {
            return true;
        }
    }
}