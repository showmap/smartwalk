﻿using System;
using System.Collections.Generic;
using MonoTouch.UIKit;
using SmartWalk.Client.iOS.Controls;
using SmartWalk.Client.iOS.Resources;
using SmartWalk.Client.iOS.Utils;

namespace SmartWalk.Client.iOS.Views.Common.Base
{
    public abstract class CustomNavBarViewBase : ViewBase
    {
        private UIBarButtonItem[] _navBarItems;
        private ButtonBarButton _backButton;
        private ButtonBarButton _moreButton;

        protected CustomNavBarViewBase()
        {
            IsBackButtonVisible = true;
            IsMoreButtonVisible = true;
        }

        protected bool IsBackButtonVisible { get; set; }
        protected bool IsMoreButtonVisible { get; set; }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            if (UIDevice.CurrentDevice.CheckSystemVersion(7, 0))
            {
                EdgesForExtendedLayout = UIRectEdge.None;
            }

            View.BackgroundColor = Theme.BackgroundPatternColor;

            InitializeCustomNavBarItems();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            if (UIDevice.CurrentDevice.CheckSystemVersion(7, 0))
            {
                UIApplication.SharedApplication
                    .SetStatusBarStyle(UIStatusBarStyle.LightContent, true);
            }

            NavBarManager.Instance.SetNavBarVisibility(false, false, true, animated);
            NavBarManager.Instance.NavBar.SetItems(_navBarItems, animated);
            ButtonBarUtil.UpdateButtonsFrameOnRotation(_navBarItems);
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            NavBarManager.Instance.NavBar.SetItems(new UIBarButtonItem[]{}, animated);
        }

        public override void WillAnimateRotation(UIInterfaceOrientation toInterfaceOrientation, double duration)
        {
            base.WillAnimateRotation(toInterfaceOrientation, duration);

            ButtonBarUtil.UpdateButtonsFrameOnRotation(_navBarItems);
        }

        public override void WillMoveToParentViewController(UIViewController parent)
        {
            base.WillMoveToParentViewController(parent);

            if (parent == null)
            {
                DisposeCustomNavBarItems();
            }
        }

        protected virtual void OnInitializeCustomNavBarItems(List<UIBarButtonItem> navBarItems)
        {
        }

        private void InitializeCustomNavBarItems()
        {
            // just in case replacing default iOS back button (it may not be visible)
            if (NavigationController.ViewControllers.Length > 1)
            {
                ButtonBarUtil.OverrideNavigatorBackButton(
                    NavigationItem, 
                    () => OnNavigationBackClick(null, EventArgs.Empty));
            }

            var navBarItems = new List<UIBarButtonItem>();
            var gap = 
                UIDevice.CurrentDevice.CheckSystemVersion(7, 0)
                    ? ButtonBarUtil.CreateGapSpacer()
                    : ButtonBarUtil.CreateGapSpacer(Theme.CustomNavBarPaddingCompensate);

            if (IsBackButtonVisible)
            {
                _backButton = ButtonBarUtil.Create(ThemeIcons.NavBarBack, ThemeIcons.NavBarBackLandscape, true);
                _backButton.TouchUpInside += OnNavigationBackClick;
                var backBarButton = new UIBarButtonItem(_backButton);

                navBarItems.AddRange(new [] { gap, backBarButton });
            }

            var space = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace);
            navBarItems.Add(space);

            OnInitializeCustomNavBarItems(navBarItems);

            if (IsMoreButtonVisible)
            {
                _moreButton = ButtonBarUtil.Create(ThemeIcons.NavBarMore, ThemeIcons.NavBarMoreLandscape, true);
                _moreButton.TouchUpInside += OnMoreButtonClicked;
                var moreBarButton = new UIBarButtonItem(_moreButton);

                navBarItems.AddRange(new [] { moreBarButton, gap });
            }

            _navBarItems = navBarItems.ToArray();
        }

        private void DisposeCustomNavBarItems()
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
            NavigationController.PopViewControllerAnimated(true);
        }

        private void OnMoreButtonClicked(object sender, EventArgs e)
        {
            ShowActionSheet();
        }
    }
}