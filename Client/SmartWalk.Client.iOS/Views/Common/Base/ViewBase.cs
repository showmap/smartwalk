using System;
using System.Collections.Generic;
using System.ComponentModel;
using Cirrious.CrossCore;
using Cirrious.CrossCore.Core;
using SmartWalk.Client.Core.Resources;
using SmartWalk.Client.Core.ViewModels;
using SmartWalk.Client.Core.ViewModels.Interfaces;
using SmartWalk.Client.iOS.Resources;
using SmartWalk.Client.iOS.Utils;
using SmartWalk.Shared.Utils;
using UIKit;

namespace SmartWalk.Client.iOS.Views.Common.Base
{
    public abstract class ViewBase : ActiveAwareViewBase
    {
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

                DisposeModalView();
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
                    SetNeedsStatusBarAppearanceUpdate);
            }
            else
            {
                SetNeedsStatusBarAppearanceUpdate();
            }
        }

        public void ShowHideModalView(ModalViewContext context)
        {
            if (PresentedViewController != null)
            {
                DisposeModalView();
            }

            if (context != null)
            {
                var view = default(IModalView);

                switch (context.ViewKind)
                {
                    case ModalViewKind.FullscreenImage:
                        view = new ImageFullscreenView {
                            ImageURL = context.DataContext as string
                        };
                        break;

                    case ModalViewKind.Browser:
                        var viewModel = Mvx.IocConstruct<BrowserViewModel>();
                        viewModel.Init(new BrowserViewModel.Parameters { 
                            URL = context.DataContext as string
                        });
                        view = new ModalNavView(new BrowserView { ViewModel = viewModel });
                        break;
                }

                view.ToHide += OnModalViewToHide;

                PresentViewController((UIViewController)view, true, null);
            }
        }

        protected virtual void UpdateViewTitle()
        {
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
            View.BackgroundColor = ThemeColors.ContentLightBackground;
        }

        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var modalProvider = ViewModel as IModalProviderViewModel;
            if (modalProvider != null &&
                e.PropertyName == modalProvider.GetPropertyName(p => p.ModalViewContext))
            {
                ShowHideModalView(modalProvider.ModalViewContext);
            }

            OnViewModelPropertyChanged(e.PropertyName);
        }

        private void OnViewModelShare(object sender, MvxValueEventArgs<string> e)
        {
            ShareUtil.Share(this, e.Value);
        }

        private void DisposeModalView()
        {
            if (PresentedViewController != null)
            {
                PresentedViewController.DismissViewController(true, null);
                ((IModalView)PresentedViewController).ToHide -= OnModalViewToHide;
                ((IModalView)PresentedViewController).Dispose();
            }
        }

        private void OnModalViewToHide(object sender, EventArgs e)
        {
            var modalProvider = ViewModel as IModalProviderViewModel;
            if (modalProvider != null &&
                modalProvider.ShowHideModalViewCommand.CanExecute(null))
            {
                modalProvider.ShowHideModalViewCommand.Execute(null);
            }
        }
    }
}