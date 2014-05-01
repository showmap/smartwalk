using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Client.Core.Model.DataContracts;
using SmartWalk.Client.Core.ViewModels.Interfaces;
using SmartWalk.Shared.Utils;
using SmartWalk.Client.iOS.Controls;
using SmartWalk.Client.iOS.Resources;
using SmartWalk.Client.iOS.Utils;
using SmartWalk.Client.iOS.Views.Common.EntityCell;

namespace SmartWalk.Client.iOS.Views.Common.Base
{
    public abstract class ListViewBase : ActiveAwareViewController
    {
        public const double ListViewShowAnimationDuration = 0.15;

        private UISwipeGestureRecognizer _swipeRight;
        private UIRefreshControl _refreshControl;
        private ListViewDecorator _listView;
        private UIView _progressView;
        private ImageFullscreenView _imageFullscreenView;
        private ContactsView _contactsView;

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

            View.BackgroundColor = Theme.BackgroundPatternColor;

            // override back button if it is visible
            if (NavigationController.ViewControllers.Length > 1)
            {
                ButtonBarUtil.OverrideNavigatorBackButton(NavigationItem, OnNavigationBackClick);
            }

            var notifyableViewModel = ViewModel as INotifyPropertyChanged;
            if (notifyableViewModel != null)
            {
                notifyableViewModel.PropertyChanged += OnViewModelPropertyChanged;
            }

            var refreshableViewModel = ViewModel as IRefreshableViewModel;
            if (refreshableViewModel != null)
            {
                InitializeRefreshControl();
                refreshableViewModel.RefreshCompleted += OnViewModelRefreshCompleted;
            }

            var shareableViewModel = ViewModel as IShareableViewModel;
            if (shareableViewModel != null)
            {
                shareableViewModel.Share += (sender, e) => ShareUtil.Share(this, e.Value);
            }

            UpdateViewTitle();
            UpdateViewState();

            InitializeConstraints();
            InitializeListView();
            InitializeGesture();
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
                DisposeContactsView();
            }
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            ButtonBarUtil.UpdateButtonsFrameOnRotation(NavigationItem.LeftBarButtonItems);
            ButtonBarUtil.UpdateButtonsFrameOnRotation(NavigationItem.RightBarButtonItems);

            if (_contactsView != null)
            {
                SetDialogViewFullscreenFrame(_contactsView);
            }
        }

        public override void WillAnimateRotation(UIInterfaceOrientation toInterfaceOrientation, double duration)
        {
            base.WillAnimateRotation(toInterfaceOrientation, duration);

            ButtonBarUtil.UpdateButtonsFrameOnRotation(NavigationItem.LeftBarButtonItems);
            ButtonBarUtil.UpdateButtonsFrameOnRotation(NavigationItem.RightBarButtonItems);

            if (_contactsView != null)
            {
                SetDialogViewFullscreenFrame(_contactsView);
            }
        }

        protected abstract ListViewDecorator GetListView();

        protected abstract UIView GetProgressViewContainer();

        protected abstract NSLayoutConstraint GetProgressViewTopConstraint();

        protected virtual void InitializeListView()
        {
            OnBeforeSetListViewSource();

            var source = CreateListViewSource();
            ListView.Source = source;
        }

        protected abstract IListViewSource CreateListViewSource();

        protected virtual void UpdateViewTitle()
        {
            NavigationItem.Title = ViewTitle ?? string.Empty;
        }

        protected virtual void OnNavigationBackClick()
        {
            NavigationController.PopViewControllerAnimated(true);
        }

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

        protected void SetDialogViewFullscreenFrame(UIView view)
        {
            if (UIDevice.CurrentDevice.CheckSystemVersion(7, 0))
            {
                view.Frame = new RectangleF(
                    View.Bounds.Left,
                    View.Bounds.Top + TopLayoutGuide.Length,
                    View.Bounds.Width, 
                    View.Bounds.Height - TopLayoutGuide.Length);
            }
            else
            {
                view.Frame = View.Bounds;
            }
        }

        private void InitializeGesture()
        {
            _swipeRight = new UISwipeGestureRecognizer(() => 
                NavigationController.PopViewControllerAnimated(true));

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
                TintColor = Theme.RefreshControl
            };

            // make sure that it's under all other views
            _refreshControl.Layer.ZPosition = -1;

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

        private void ShowHideContactsView(Entity entity)
        {
            var contactsProvider = ViewModel as IContactsEntityProvider;
            if (entity != null && contactsProvider != null)
            {
                _contactsView = View.Subviews.OfType<ContactsView>().FirstOrDefault();
                if (_contactsView == null)
                {
                    InitializeContactsView(contactsProvider);

                    _contactsView.Alpha = 0;
                    View.Add(_contactsView);
                    UIView.BeginAnimations(null);
                    _contactsView.Alpha = 1;
                    UIView.CommitAnimations();
                }

                _contactsView.Entity = entity;
            }
            else if (_contactsView != null)
            {
                UIView.Animate(
                    0.2, 
                    new NSAction(() => _contactsView.Alpha = 0),
                    new NSAction(_contactsView.RemoveFromSuperview));

                DisposeContactsView();
            }
        }

        private void InitializeContactsView(IContactsEntityProvider contactsProvider)
        {
            _contactsView = ContactsView.Create();

            SetDialogViewFullscreenFrame(_contactsView);

            _contactsView.CloseCommand = contactsProvider.ShowHideContactsCommand;
            _contactsView.CallPhoneCommand = contactsProvider.CallPhoneCommand;
            _contactsView.ComposeEmailCommand = contactsProvider.ComposeEmailCommand;
            _contactsView.NavigateWebSiteCommand = contactsProvider.NavigateWebLinkCommand;
        }

        private void DisposeContactsView()
        {
            if (_contactsView != null)
            {
                _contactsView.CloseCommand = null;
                _contactsView.CallPhoneCommand = null;
                _contactsView.ComposeEmailCommand = null;
                _contactsView.NavigateWebSiteCommand = null;
                _contactsView.Dispose();
                _contactsView = null;
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
                ShowHideContactsView(contactsProvider.CurrentContactsEntityInfo);
            }

            var progressViewModel = ViewModel as IProgressViewModel;
            if (progressViewModel != null &&
                e.PropertyName == progressViewModel.GetPropertyName(p => p.IsLoading))
            {
                UpdateViewState();
            }

            var refreshableViewModel = ViewModel as IRefreshableViewModel;
            if (refreshableViewModel != null &&
                e.PropertyName == refreshableViewModel.GetPropertyName(p => p.Title))
            {
                UpdateViewTitle();
            }

            OnViewModelPropertyChanged(e.PropertyName);
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

        private void OnFullscreenViewHidden(object sender, EventArgs e)
        {
            var fullscreenProvider = ViewModel as IFullscreenImageProvider;
            if (fullscreenProvider != null &&
                fullscreenProvider.ShowHideFullscreenImageCommand.CanExecute(null))
            {
                fullscreenProvider.ShowHideFullscreenImageCommand.Execute(null);
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