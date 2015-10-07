using System;
using System.Collections.Generic;
using UIKit;
using SmartWalk.Client.iOS.Controls;
using SmartWalk.Client.iOS.Resources;
using SmartWalk.Client.iOS.Utils;

namespace SmartWalk.Client.iOS.Views.Common.Base
{
    public abstract class NavBarViewBase : ViewBase
    {
        private ButtonBarButton _backButton;
        private ButtonBarButton _moreButton;

        protected NavBarViewBase()
        {
            IsBackButtonVisible = true;
            IsMoreButtonVisible = true;
        }

        protected bool IsBackButtonVisible { get; set; }
        protected bool IsMoreButtonVisible { get; set; }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            InitializeNavBarItems();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            SetNavBarHidden(false, animated);
            SetNavBarTransparent(SemiTransparentType.Light, animated);
        }

        public override void DidMoveToParentViewController(UIViewController parent)
        {
            base.DidMoveToParentViewController(parent);

            if (parent == null)
            {
                DisposeNavBarItems();
            }
        }

        protected override void UpdateViewTitle()
        {
            if (NavBarManager.Instance.IsTransparent)
            {
                NavigationItem.Title = string.Empty;
            }
            else
            {

                NavigationItem.Title = ViewTitle ?? string.Empty;
            }
        }

        protected virtual void SetNavBarHidden(bool hidden, bool animated)
        {
            NavBarManager.Instance.SetHidden(hidden, animated);
        }

        protected virtual void SetNavBarTransparent(SemiTransparentType transparentType, bool animated)
        {
            NavBarManager.Instance.SetTransparentType(transparentType, animated);
            UpdateViewTitle();
        }

        protected virtual void OnInitializeNavBarItems(List<UIBarButtonItem> navBarItems)
        {
        }

        private void InitializeNavBarItems()
        {
            var navBarItems = new List<UIBarButtonItem>();
            var gap = ButtonBarUtil.CreateGapSpacer();

            // replacing default iOS back button
            if (IsBackButtonVisible &&
                NavigationController != null &&
                NavigationController.ViewControllers.Length > 1)
            {
                NavigationItem.HidesBackButton = true;

                _backButton = ButtonBarUtil.Create(
                    ThemeIcons.Back, 
                    ThemeIcons.BackLandscape, 
                    SemiTransparentType.Light);
                _backButton.TouchUpInside += OnNavigationBackClick;
                _backButton.UpdateState();

                var backBarButton = new UIBarButtonItem(_backButton);
                NavigationItem.SetLeftBarButtonItems(new [] { gap, backBarButton }, true);
            }

            OnInitializeNavBarItems(navBarItems);

            // More (...) Button
            if (IsMoreButtonVisible)
            {
                _moreButton = ButtonBarUtil.Create(
                    ThemeIcons.More, 
                    ThemeIcons.MoreLandscape, 
                    SemiTransparentType.Light);
                _moreButton.TouchUpInside += OnMoreButtonClicked;

                var moreBarButton = new UIBarButtonItem(_moreButton);
                navBarItems.AddRange(new [] { moreBarButton, gap });
            }

            navBarItems.Reverse();

            NavigationItem.SetRightBarButtonItems(navBarItems.ToArray(), true);
        }

        private void DisposeNavBarItems()
        {
            if (_backButton != null)
            {
                _backButton.TouchUpInside -= OnNavigationBackClick;
            }

            if (_moreButton != null)
            {
                _moreButton.TouchUpInside -= OnMoreButtonClicked;
            }
        }

        private void OnNavigationBackClick(object sender, EventArgs e)
        {
            NavigationController.PopViewController(true);
        }

        private void OnMoreButtonClicked(object sender, EventArgs e)
        {
            ShowActionSheet();
        }
    }
}