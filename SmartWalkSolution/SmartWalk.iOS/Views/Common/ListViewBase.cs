using System;
using System.ComponentModel;
using Cirrious.MvvmCross.Touch.Views;
using MonoTouch.UIKit;
using SmartWalk.Core.ViewModels;
using SmartWalk.iOS.Controls;

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
        }

        protected abstract ListViewDecorator GetListView();

        protected virtual void UpdateViewTitle()
        {
        }

        protected virtual void InitializeListView()
        {
            _swipeRight = new UISwipeGestureRecognizer(rec => 
                {
                    NavigationController.PopViewControllerAnimated(true);
                });

            _swipeRight.Direction = UISwipeGestureRecognizerDirection.Right;
            ListView.AddGestureRecognizer(_swipeRight);

            var source = CreateListViewSource();

            ListView.Source = source;

            _refreshControl = new UIRefreshControl
                {
                    TintColor = ThemeColors.Mercury
                };
            _refreshControl.ValueChanged += (sender, e) => 
                {
                    if (ViewModel.RefreshCommand.CanExecute(null))
                    {
                        ViewModel.RefreshCommand.Execute(null);
                    }
                };

            ListView.AddSubview(_refreshControl);
        }

        protected abstract object CreateListViewSource();

        protected virtual void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
        }

        protected void ShowImageFullscreenView(string url)
        {
            if (_imageFullscreenView == null)
            {
                _imageFullscreenView = new ImageFullscreenView();
            }

            _imageFullscreenView.ImageURL = url;
            _imageFullscreenView.Show();
        }

        protected override void Dispose(bool disposing)
        {
            if (_refreshControl != null)
            {
                _refreshControl.Dispose();
                _refreshControl = null;
            }

            if (_swipeRight != null)
            {
                _swipeRight.Dispose();
                _swipeRight = null;
            }

            if (_imageFullscreenView != null)
            {
                _imageFullscreenView.Dispose();
                _imageFullscreenView = null;
            }

            base.Dispose(disposing);
        }

        private void OnViewModelRefreshCompleted(object sender, EventArgs e)
        {
            UpdateViewTitle();
            InvokeOnMainThread(_refreshControl.EndRefreshing);
        }
    }
}