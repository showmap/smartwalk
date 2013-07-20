using System;
using System.ComponentModel;
using Cirrious.MvvmCross.Binding.Touch.Views;
using Cirrious.MvvmCross.Touch.Views;
using MonoTouch.UIKit;
using SmartWalk.Core.ViewModels;

namespace SmartWalk.iOS.Views.Common
{
    public abstract class TableViewBase : MvxViewController
    {
        private UIRefreshControl _refreshControl;

        public new IRefreshableViewModel ViewModel
        {
            get { return (IRefreshableViewModel)base.ViewModel; }
            set { base.ViewModel = value; }
        }

        public abstract UITableView TableView { get; }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            ViewModel.PropertyChanged += OnViewModelPropertyChanged;
            ViewModel.RefreshCompleted += OnViewModelRefreshCompleted;

            UpdateViewTitle();

            InitializeTableView();
        }

        protected virtual void UpdateViewTitle()
        {
        }

        protected virtual void InitializeTableView()
        {
            var tableSource = CreateTableViewSource();

            TableView.Source = tableSource;
            TableView.ReloadData();

            _refreshControl = new UIRefreshControl();
            _refreshControl.ValueChanged += (sender, e) => 
                {
                    if (ViewModel.RefreshCommand.CanExecute(null))
                    {
                        ViewModel.RefreshCommand.Execute(null);
                    }
                };

            TableView.AddSubview(_refreshControl);
        }

        protected abstract MvxTableViewSource CreateTableViewSource();

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