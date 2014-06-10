using System;
using System.ComponentModel;
using System.Drawing;
using Cirrious.CrossCore.Core;
using MonoTouch.UIKit;
using SmartWalk.Client.Core.ViewModels.Interfaces;
using SmartWalk.Shared.Utils;
using SmartWalk.Client.iOS.Resources;
using SmartWalk.Client.iOS.Utils;

namespace SmartWalk.Client.iOS.Views.Common.Base
{
    public abstract class ViewBase : ActiveAwareViewBase
    {
        private UISwipeGestureRecognizer _swipeRight;
        private ImageFullscreenView _imageFullscreenView;

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
            InitializeStyle();
        }

        public override void DidMoveToParentViewController(UIViewController parent)
        {
            base.DidMoveToParentViewController(parent);

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
            }
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            ButtonBarUtil.UpdateButtonsFrameOnRotation(NavigationItem.LeftBarButtonItems);
            ButtonBarUtil.UpdateButtonsFrameOnRotation(NavigationItem.RightBarButtonItems);
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            if (UIDevice.CurrentDevice.CheckSystemVersion(7, 0))
            {
                NavigationController.InteractivePopGestureRecognizer.Enabled = true;
                NavigationController.InteractivePopGestureRecognizer.WeakDelegate = this;
            }
        }

        public override void WillAnimateRotation(UIInterfaceOrientation toInterfaceOrientation, double duration)
        {
            base.WillAnimateRotation(toInterfaceOrientation, duration);

            ButtonBarUtil.UpdateButtonsFrameOnRotation(NavigationItem.LeftBarButtonItems);
            ButtonBarUtil.UpdateButtonsFrameOnRotation(NavigationItem.RightBarButtonItems);
        }

        protected virtual void SetStatusBarHidden(bool hidden, bool animated)
        {
            UIApplication.SharedApplication.SetStatusBarHidden(
                hidden, 
                animated ? UIStatusBarAnimation.Slide : UIStatusBarAnimation.None);
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
            // in iOS 7 using built-in pop gesture recognizer
            if (!UIDevice.CurrentDevice.CheckSystemVersion(7, 0))
            {
                _swipeRight = new UISwipeGestureRecognizer(() => 
                NavigationController.PopViewControllerAnimated(true));

                _swipeRight.Direction = UISwipeGestureRecognizerDirection.Right;

                View.AddGestureRecognizer(_swipeRight);
            }
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

        private void InitializeStyle()
        {
            View.BackgroundColor = Theme.BackgroundPatternColor;

            if (UIDevice.CurrentDevice.CheckSystemVersion(7, 0))
            {
                EdgesForExtendedLayout = UIRectEdge.None;
                UIApplication.SharedApplication
                    .SetStatusBarStyle(UIStatusBarStyle.Default, false);
            }
            else
            {
                #pragma warning disable 618

                WantsFullScreenLayout = true;
                UIApplication.SharedApplication
                    .SetStatusBarStyle(UIStatusBarStyle.BlackTranslucent, false);

                #pragma warning restore 618
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