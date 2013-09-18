using System;
using System.ComponentModel;
using Cirrious.MvvmCross.Touch.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Core.Model;
using SmartWalk.Core.Utils;
using SmartWalk.Core.ViewModels.Interfaces;
using SmartWalk.iOS.Controls;
using SmartWalk.iOS.Utils;
using SmartWalk.iOS.Views.Common.EntityCell;

namespace SmartWalk.iOS.Views.Common
{
    public abstract class ListViewBase : MvxViewController
    {
        public const double ListViewShowAnimationDuration = 0.15;

        private UISwipeGestureRecognizer _swipeRight;
        private UIRefreshControl _refreshControl;
        private ListViewDecorator _listView;
        private UIView _progressView;
        private ImageFullscreenView _imageFullscreenView;

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

            View.BackgroundColor = UIColor.FromPatternImage(UIImage.FromFile("Images/Background.png"));

            InitializeLeftButton();

            var notifyableViewModel = ViewModel as INotifyPropertyChanged;
            if (notifyableViewModel != null)
            {
                notifyableViewModel.PropertyChanged += OnViewModelPropertyChanged;
            }

            var refreshableViewModel = ViewModel as IRefreshableViewModel;
            if (refreshableViewModel != null)
            {
                refreshableViewModel.RefreshCompleted += OnViewModelRefreshCompleted;
            }

            UpdateViewTitle();
            UpdateViewState();

            InitializeListView();
            InitializeGesture();

            if (refreshableViewModel != null)
            {
                InitializeRefreshControl();
            }
        }

        public override void WillMoveToParentViewController(UIViewController parent)
        {
            base.WillMoveToParentViewController(parent);

            if (parent == null)
            {
                var notifyableViewModel = ViewModel as INotifyPropertyChanged;
                if (notifyableViewModel != null)
                {
                    notifyableViewModel.PropertyChanged -= OnViewModelPropertyChanged;
                }

                var refreshableViewModel = ViewModel as IRefreshableViewModel;
                if (refreshableViewModel != null)
                {
                    refreshableViewModel.RefreshCompleted -= OnViewModelRefreshCompleted;
                }

                DisposeGesture();
                DisposeRefreshControl();
                DisposeFullscreenView();
            }
        }

        protected abstract ListViewDecorator GetListView();

        protected abstract UIView GetProgressViewContainer();

        protected virtual void InitializeLeftButton()
        {
            NavigationItem.HidesBackButton = true;

            var spacer = ButtonBarUtil.CreateSpacer();

            var button = ButtonBarUtil.Create("Icons/NavBarBack.png");
            button.TouchUpInside += (sender, e) => 
                NavigationController.PopViewControllerAnimated(true);
            var barButton = new UIBarButtonItem(button);

            NavigationItem.SetLeftBarButtonItems(new [] { spacer, barButton }, true);
        }

        protected virtual string GetViewTitle()
        {
            return null;
        }

        protected virtual void InitializeListView()
        {
            var source = CreateListViewSource();

            OnBeforeSetListViewSource();

            ListView.Source = source;
        }

        protected abstract IListViewSource CreateListViewSource();

        protected virtual void OnBeforeSetListViewSource()
        {
        }

        protected virtual void OnViewModelPropertyChanged(string propertyName)
        {
        }

        protected virtual void OnViewModelRefreshed()
        {
        }

        protected virtual void OnLoadingViewStateUpdate()
        {
            ListView.View.Hidden = true;
        }

        protected virtual void OnLoadedViewStateUpdate()
        {
            UIView.Transition(
                ListView.View,
                ListViewShowAnimationDuration,
                UIViewAnimationOptions.TransitionCrossDissolve,
                new NSAction(() => ListView.View.Hidden = false),
                null);
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
                _imageFullscreenView.Hide();
                _imageFullscreenView.Dispose();
                _imageFullscreenView = null;
            }
        }

        private void ShowContactsView(EntityInfo entityInfo)
        {
            if (entityInfo != null)
            {
                var contactsView = ContactsView.Create();
                contactsView.Close += OnContactsViewClose;
                contactsView.Frame = View.Bounds;
                contactsView.EntityInfo = entityInfo;
                contactsView.NavigateWebSiteCommand = 
                ((IContactsEntityProvider)ViewModel).NavigateWebLinkCommand;

                contactsView.Alpha = 0;
                View.Add(contactsView);
                UIView.BeginAnimations(null);
                contactsView.Alpha = 1;
                UIView.CommitAnimations();
            }
        }

        private void OnContactsViewClose(object sender, EventArgs e)
        {
            var contactsView = (ContactsView)sender;

            UIView.Animate(
                0.2, 
                new NSAction(() => contactsView.Alpha = 0),
                new NSAction(contactsView.RemoveFromSuperview));

            contactsView.Close -= OnContactsViewClose;
            var contactsProvider = ((IContactsEntityProvider)ViewModel);

            if (contactsProvider.ShowHideContactsCommand.CanExecute(null))
            {
                contactsProvider.ShowHideContactsCommand.Execute(null);
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

            var contactsProvider = ViewModel as IContactsEntityProvider;
            if (contactsProvider != null &&
                e.PropertyName == contactsProvider.GetPropertyName(p => p.CurrentContactsEntityInfo))
            {
                ShowContactsView(contactsProvider.CurrentContactsEntityInfo);
            }

            var progressViewModel = ViewModel as IProgressViewModel;
            if (progressViewModel != null &&
                e.PropertyName == progressViewModel.GetPropertyName(p => p.IsLoading))
            {
                UpdateViewState();
            }

            OnViewModelPropertyChanged(e.PropertyName);
        }

        private void OnViewModelRefreshCompleted(object sender, EventArgs e)
        {
            UpdateViewTitle();
            InvokeOnMainThread(_refreshControl.EndRefreshing);

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

        private void OnFullscreenViewHidden(object sender, EventArgs e)
        {
            var fullscreenProvider = ViewModel as IFullscreenImageProvider;
            if (fullscreenProvider != null &&
                fullscreenProvider.ShowHideFullscreenImageCommand.CanExecute(null))
            {
                fullscreenProvider.ShowHideFullscreenImageCommand.Execute(null);
            }   
        }

        // TODO: probably upper case only of EN locale
        private void UpdateViewTitle()
        {
            var title = GetViewTitle();
            NavigationItem.Title = title != null ? title.ToUpper() : string.Empty;
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