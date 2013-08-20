using System;
using System.ComponentModel;
using Cirrious.MvvmCross.Touch.Views;
using MonoTouch.UIKit;
using SmartWalk.Core.ViewModels;
using SmartWalk.iOS.Controls;
using SmartWalk.Core.Utils;
using SmartWalk.Core.ViewModels.Interfaces;

namespace SmartWalk.iOS.Views.Common
{
    public abstract class ListViewBase : MvxViewController
    {
        private UISwipeGestureRecognizer _swipeRight;
        private UIRefreshControl _refreshControl;
        private ListViewDecorator _listView;
        private ImageFullscreenView _imageFullscreenView;

        public new IRefreshableViewModel ViewModel
        {
            get { return (IRefreshableViewModel)base.ViewModel; }
        }

        public ListViewDecorator ListView 
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

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            ViewModel.PropertyChanged += OnViewModelPropertyChanged;
            ViewModel.RefreshCompleted += OnViewModelRefreshCompleted;

            UpdateViewTitle();

            InitializeListView();
            InitializeGesture();
            InitializeRefreshControl();
        }

        public override void WillMoveToParentViewController(UIViewController parent)
        {
            base.WillMoveToParentViewController(parent);

            if (parent == null)
            {
                ViewModel.PropertyChanged -= OnViewModelPropertyChanged;
                ViewModel.RefreshCompleted -= OnViewModelRefreshCompleted;

                DisposeGesture();
                DisposeRefreshControl();
                DisposeFullscreenView();
            }
        }

        protected abstract ListViewDecorator GetListView();

        protected virtual void UpdateViewTitle()
        {
        }

        protected virtual void InitializeListView()
        {
            var source = CreateListViewSource();

            ListView.Source = source;
        }

        protected abstract object CreateListViewSource();

        protected virtual void OnViewModelPropertyChanged(string propertyName)
        {
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            ConsoleUtil.LogDisposed(this);
        }

        private void InitializeGesture()
        {
            _swipeRight = new UISwipeGestureRecognizer(() => {
                NavigationController.PopViewControllerAnimated(true);
            });

            _swipeRight.Direction = UISwipeGestureRecognizerDirection.Right;

            ListView.AddGestureRecognizer(_swipeRight);
        }

        private void DisposeGesture()
        {
            if (_swipeRight != null)
            {
                ListView.RemoveGestureRecognizer(_swipeRight);
                _swipeRight.Dispose();
                _swipeRight = null;
            }
        }

        private void InitializeRefreshControl()
        {
            _refreshControl = new UIRefreshControl {
                TintColor = ThemeColors.Mercury
            };

            _refreshControl.ValueChanged += OnRefreshControlValueChanged;

            ListView.AddSubview(_refreshControl);
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

        private void ShowHideImageFullscreenView(string url)
        {
            if (_imageFullscreenView != null)
            {
                DisposeFullscreenView();
            }

            if (url != null)
            {
                _imageFullscreenView = new ImageFullscreenView
                {
                    ImageURL = url
                };
                _imageFullscreenView.Hidden += OnFullscreenViewHidden;

                _imageFullscreenView.Show();
            }
        }

        private void DisposeFullscreenView()
        {
            if (_imageFullscreenView != null)
            {
                _imageFullscreenView.Hidden -= OnFullscreenViewHidden;
                _imageFullscreenView.Dispose();
                _imageFullscreenView = null;
            }
        }

        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var fullscreenProvider = ViewModel as IFullscreenImageProvider;
            if (fullscreenProvider != null &&
                e.PropertyName == fullscreenProvider.GetPropertyName(p => p.CurrentFullscreenImage))
            {
                ShowHideImageFullscreenView(fullscreenProvider.CurrentFullscreenImage);
            }

            OnViewModelPropertyChanged(e.PropertyName);
        }

        private void OnViewModelRefreshCompleted(object sender, EventArgs e)
        {
            UpdateViewTitle();
            InvokeOnMainThread(_refreshControl.EndRefreshing);
        }

        private void OnRefreshControlValueChanged(object sender, EventArgs e)
        {
            if (ViewModel.RefreshCommand.CanExecute(null))
            {
                ViewModel.RefreshCommand.Execute(null);
            }
        }

        private void OnFullscreenViewHidden(object sender, EventArgs e)
        {
            var fullscreenProvider = ViewModel as IFullscreenImageProvider;
            if (fullscreenProvider != null &&
                fullscreenProvider.ShowHideFullscreenImageCommand.CanExecute(null))
            {
                fullscreenProvider.ShowHideFullscreenImageCommand.Execute(null);
            }
        }
    }
}