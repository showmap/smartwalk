using System;
using System.Linq;
using System.ComponentModel;
using Cirrious.MvvmCross.Touch.Views;
using MonoTouch.UIKit;
using SmartWalk.Core.ViewModels;
using SmartWalk.iOS.Controls;

namespace SmartWalk.iOS.Views.Common
{
    public abstract class ListViewBase : MvxViewController
    {
        private UIRefreshControl _refreshControl;
        private ListViewDecorator _listView;

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
            var swipeRight = new UISwipeGestureRecognizer(rec => 
                {
                    NavigationController.PopViewControllerAnimated(true);
                });

            swipeRight.Direction = UISwipeGestureRecognizerDirection.Right;
            ListView.AddGestureRecognizer(swipeRight);

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

        private void OnViewModelRefreshCompleted(object sender, EventArgs e)
        {
            UpdateViewTitle();
            InvokeOnMainThread(_refreshControl.EndRefreshing);
        }
    }
}