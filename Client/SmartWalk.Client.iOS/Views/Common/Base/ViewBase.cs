using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using Cirrious.CrossCore.Core;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Client.Core.Model.DataContracts;
using SmartWalk.Client.Core.ViewModels.Interfaces;
using SmartWalk.Shared.Utils;
using SmartWalk.Client.iOS.Resources;
using SmartWalk.Client.iOS.Utils;
using SmartWalk.Client.iOS.Views.Common.EntityCell;

namespace SmartWalk.Client.iOS.Views.Common.Base
{
    public abstract class ViewBase : ActiveAwareViewController
    {
        private UISwipeGestureRecognizer _swipeRight;
        private ImageFullscreenView _imageFullscreenView;
        private ContactsView _contactsView;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var notifyableViewModel = ViewModel as INotifyPropertyChanged;
            if (notifyableViewModel != null)
            {
                notifyableViewModel.PropertyChanged += OnViewModelPropertyChanged;
            }

            var shareableViewModel = ViewModel as IShareableViewModel;
            if (shareableViewModel != null)
            {
                shareableViewModel.Share += OnViewModelShare;
            }

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

                var shareableViewModel = ViewModel as IShareableViewModel;
                if (shareableViewModel != null)
                {
                    shareableViewModel.Share -= OnViewModelShare;
                }

                DisposeGesture();
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

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            ConsoleUtil.LogDisposed(this);
        }

        protected virtual void OnViewModelPropertyChanged(string propertyName)
        {
        }

        protected void SetDialogViewFullscreenFrame(UIView view)
        {
            if (UIDevice.CurrentDevice.CheckSystemVersion(7, 0) &&
                !NavigationController.NavigationBarHidden)
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

        protected void ShowActionSheet()
        {
            var actionSheet = ActionSheetUtil.CreateActionSheet(OnActionClicked);

            OnInitializingActionSheet(actionSheet);

            actionSheet.AddButton(Localization.CancelButton);
            actionSheet.CancelButtonIndex = actionSheet.ButtonCount - 1;

            actionSheet.ShowInView(View);
        }

        protected virtual void OnInitializingActionSheet(UIActionSheet actionSheet)
        {
        }

        protected virtual void OnActionSheetClick(string buttonTitle)
        {
        }

        private void OnActionClicked(object sender, UIButtonEventArgs e)
        {
            var actionSheet = ((UIActionSheet)sender);
            actionSheet.Clicked -= OnActionClicked;

            OnActionSheetClick(actionSheet.ButtonTitle(e.ButtonIndex));
        }

        private void InitializeGesture()
        {
            _swipeRight = new UISwipeGestureRecognizer(() => 
                NavigationController.PopViewControllerAnimated(true));

            _swipeRight.Direction = UISwipeGestureRecognizerDirection.Right;

            View.AddGestureRecognizer(_swipeRight);
        }

        private void DisposeGesture()
        {
            if (_swipeRight != null)
            {
                View.RemoveGestureRecognizer(_swipeRight);
                _swipeRight.Dispose();
                _swipeRight = null;
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

            OnViewModelPropertyChanged(e.PropertyName);
        }

        private void OnViewModelShare(object sender, MvxValueEventArgs<string> e)
        {
            ShareUtil.Share(this, e.Value);
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