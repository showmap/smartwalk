using System;
using Cirrious.CrossCore.Core;
using MonoTouch.UIKit;
using SmartWalk.Client.Core.ViewModels.Interfaces;
using SmartWalk.Shared.Utils;
using SmartWalk.Client.iOS.Controls;
using SmartWalk.Client.iOS.Resources;
using SmartWalk.Client.iOS.Utils;

namespace SmartWalk.Client.iOS.Views.Common.Base
{
    public abstract class ListViewBase : NavBarViewBase
    {
        private ListViewDecorator _listView;
        private UIView _progressViewContainer;
        private ProgressView _progressView;
        private IListViewSource _listViewSource;
        private UITapGestureRecognizer _viewTapGesture;

        protected bool IsLoading
        {
            get
            { 
                return 
                    ViewModel is IProgressViewModel &&
                    ((IProgressViewModel)ViewModel).IsLoading;
            }
        }

        protected ProgressView ProgressView
        {
            get { return _progressView; }
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
                return 
                    ListView.Source != null && 
                    ListView.Source.ItemsSource != null;
            }
        }

        private bool IsRefreshingAfterPull
        {
            get
            {
                return 
                    ListView != null &&
                    ListView.RefreshControl != null &&
                    ListView.RefreshControl.Refreshing;
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

            InitializeListView();
            InitializeProgressView();
            InitializeGesture();

            UpdateViewLoadingState();
            SetNeedStatusBarUpdate(true);
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
                DisposeListView();
                DisposeGesture();
            }
        }

        public override bool PrefersStatusBarHidden()
        {
            var visible = 
                (IsLoading && !HasListData) || 
                (IsLoading && IsRefreshingAfterPull);
            return !visible;
        }

        protected abstract ListViewDecorator GetListView();

        protected abstract UIView GetProgressViewContainer();

        protected virtual void InitializeListView()
        {
            OnBeforeSetListViewSource();

            _listViewSource = CreateListViewSource();
            ListView.Source = _listViewSource;
        }

        protected abstract IListViewSource CreateListViewSource();

        protected virtual void OnBeforeSetListViewSource()
        {
        }

        protected virtual void OnViewModelRefreshed(bool hasData, bool pullToRefresh)
        {
            if (hasData && pullToRefresh)
            {
                ScrollViewToTop();
            }
        }

        protected virtual void ScrollViewToTop()
        {
            ListView.View.SetActualContentOffset(0, true);

            if (ListView != null &&
                ListView.Source != null)
            {
                ListView.Source.ScrolledToTop(ListView.View);
            }
        }

        protected virtual void OnLoadingViewStateUpdate()
        {
            // hiding ListView on view opening if data is being loaded
            // to avoid Pull-To-Refreshes and to have fade id effect
            if (!HasListData && !IsRefreshingAfterPull)
            {
                ListView.View.Hidden = true;
            }
        }

        protected virtual void OnLoadedViewStateUpdate()
        {
            if (ListView.View.Hidden)
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
                SetNeedStatusBarUpdate(true);
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
            _viewTapGesture = new UITapGestureRecognizer(ScrollViewToTop) {
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

        private void DisposeListView()
        {
            ListView.Dispose();
        }

        private void InitializeRefreshControl()
        {
            var refreshControl = new UIRefreshControl {
                TintColor = Theme.RefreshControl
            };

            refreshControl.Layer.ZPosition = -100; // HACK: Trying to put it behind table/collection header
            refreshControl.ValueChanged += OnRefreshControlValueChanged;

            ListView.RefreshControl = refreshControl;
        }

        private void DisposeRefreshControl()
        {
            if (ListView.RefreshControl != null)
            {
                ListView.RefreshControl.ValueChanged -= OnRefreshControlValueChanged;
                ListView.RefreshControl.Dispose();
                ListView.RefreshControl = null;
            }
        }

        private void OnRefreshCompleted(object sender, MvxValueEventArgs<bool> e)
        {
            var pullToRefresh = 
                ListView.RefreshControl != null &&
                ListView.RefreshControl.Refreshing;

            if (pullToRefresh)
            {
                ListView.RefreshControl.EndRefreshing();
            }

            UpdateViewDataState(e.Value);
            OnViewModelRefreshed(e.Value, pullToRefresh);
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