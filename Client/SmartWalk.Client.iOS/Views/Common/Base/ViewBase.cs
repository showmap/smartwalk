using System;
using System.Collections.Generic;
using System.ComponentModel;
using Cirrious.CrossCore.Core;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Client.Core.Resources;
using SmartWalk.Client.Core.ViewModels.Interfaces;
using SmartWalk.Shared.Utils;
using SmartWalk.Client.iOS.Resources;
using SmartWalk.Client.iOS.Utils;

namespace SmartWalk.Client.iOS.Views.Common.Base
{
    public abstract class ViewBase : ActiveAwareViewBase
    {
        private ImageFullscreenView _imageFullscreenView;

        public override UIStatusBarAnimation PreferredStatusBarUpdateAnimation
        {
            get { return UIStatusBarAnimation.Slide; }
        }

        protected virtual string ViewTitle
        {
            get
            {
                var refreshableViewModel = ViewModel as ITitleAware;
                return refreshableViewModel != null ? refreshableViewModel.Title : null;
            }
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            AutomaticallyAdjustsScrollViewInsets = false;
            EdgesForExtendedLayout = UIRectEdge.None;

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

            InitializeStyle();
            UpdateViewTitle();
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

                DisposeFullscreenView();
            }
        }

        public override UIStatusBarStyle PreferredStatusBarStyle()
        {
            return UIStatusBarStyle.Default;
        }

        public override bool PrefersStatusBarHidden()
        {
            return true;
        }

        public void SetNeedStatusBarUpdate(bool animated)
        {
            if (animated)
            {
                UIView.Animate(
                    UIConstants.AnimationLongerDuration, 
                    new NSAction(SetNeedsStatusBarAppearanceUpdate));
            }
            else
            {
                SetNeedsStatusBarAppearanceUpdate();
            }
        }

        protected virtual void UpdateViewTitle()
        {
            NavigationItem.Title = ViewTitle ?? string.Empty;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            ConsoleUtil.LogDisposed(this);
        }

        protected virtual void OnViewModelPropertyChanged(string propertyName)
        {
            var refreshableViewModel = ViewModel as ITitleAware;
            if (refreshableViewModel != null &&
                propertyName == refreshableViewModel.GetPropertyName(p => p.Title))
            {
                UpdateViewTitle();
            }
        }

        protected void SetDialogViewFullscreenFrame(UIView view)
        {
            view.Frame = View.Bounds;
        }

        protected void ShowActionSheet()
        {
            if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            {
                var actionSheet = ActionSheetUtil.CreateActionSheet();
                var titles = new List<string>();

                OnInitializingActionSheet(titles);

                foreach (var title in titles)
                {
                    var action = 
                        UIAlertAction.Create(
                            title,
                            UIAlertActionStyle.Default,
                            a => OnActionSheetClick(a.Title));
                    actionSheet.AddAction(action);
                }

                var cancelAction = 
                    UIAlertAction.Create(
                        Localization.CancelButton,
                        UIAlertActionStyle.Cancel,
                        action => actionSheet.DismissViewController(true, null));
                actionSheet.AddAction(cancelAction);

                PresentViewController(actionSheet, true, null);
            }
            else
            {
                var actionSheet = ActionSheetUtil.CreateActionSheet(OnActionClicked);
                var titles = new List<string>();

                OnInitializingActionSheet(titles);

                titles.Add(Localization.CancelButton);

                foreach (var title in titles)
                {
                    actionSheet.AddButton(title);
                }

                actionSheet.CancelButtonIndex = actionSheet.ButtonCount - 1;
                actionSheet.ShowInView(View);
            }
        }

        protected virtual void OnInitializingActionSheet(List<string> titles)
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

        private void InitializeStyle()
        {
            View.BackgroundColor = Theme.BackgroundPatternColor;
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