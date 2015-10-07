using System;
using System.Collections.Generic;
using CoreGraphics;
using Foundation;
using UIKit;
using SmartWalk.Client.Core.Resources;
using SmartWalk.Client.Core.ViewModels;
using SmartWalk.Shared.Utils;
using SmartWalk.Client.iOS.Resources;
using SmartWalk.Client.iOS.Utils;
using SmartWalk.Client.iOS.Views.Common.Base;
using SmartWalk.Client.iOS.Controls;

namespace SmartWalk.Client.iOS.Views.Common
{
    public partial class BrowserView : ViewBase, IModalView
    {
        private const string DocTitle = "document.title";

        private UIActivityIndicatorView _indicatorView;
        private ButtonBarButton _closeButton;
        private ButtonBarButton _backButton;
        private ButtonBarButton _forwardButton;
        private ButtonBarButton _moreButton;
        private UIBarButtonItem _backButtonItem;
        private UIBarButtonItem _forwardButtonItem;
        private UIBarButtonItem _progressButtonItem;

        public event EventHandler ToHide;

        public new BrowserViewModel ViewModel
        {
            get { return (BrowserViewModel)base.ViewModel; }
            set { base.ViewModel = value; }
        }

        public override bool PrefersStatusBarHidden()
        {
            return NavigationController.NavigationBarHidden;
        }

        ViewBase IModalView.PresentingViewController
        {
            get { return (ViewBase)PresentingViewController; }
        }

        protected override string ViewTitle
        {
            get
            {
                var pageTitle = WebView.EvaluateJavascript(DocTitle);
                var result = !string.IsNullOrEmpty(pageTitle)
                    ? pageTitle
                    : (WebView.CanGoBack ? string.Empty : ViewModel.BrowserURL);
                return result;
            }
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            AutomaticallyAdjustsScrollViewInsets = true;
            EdgesForExtendedLayout = UIRectEdge.All;

            InitializeStyle();
            InitializeNavBarItems();
            UpdateViewTitle();

            WebView.LoadStarted += OnWebViewLoadStarted;
            WebView.LoadError += OnWebViewLoadFinished;
            WebView.LoadFinished += OnWebViewLoadFinished;

            LoadURL();
            UpdateNavButtonsState();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            UpdateButtonsFrameOnRotation();
            UpdateNavButtonsState();
        }

        public override void WillAnimateRotation(
            UIInterfaceOrientation toInterfaceOrientation, 
            double duration)
        {
            base.WillAnimateRotation(toInterfaceOrientation, duration);

            UpdateButtonsFrameOnRotation();
            UpdateNavButtonsState();
        }

        public override void DidMoveToParentViewController(UIViewController parent)
        {
            base.DidMoveToParentViewController(parent);

            if (parent == null)
            {
                DisposeNavBarItems();
                DisposeWebView();
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            ConsoleUtil.LogDisposed(this);

            DisposeNavBarItems();
            DisposeWebView();
        }

        protected override void UpdateViewTitle()
        {
            NavigationItem.Title = ViewTitle ?? string.Empty;
        }

        protected override void OnViewModelPropertyChanged(string propertyName)
        {
            base.OnViewModelPropertyChanged(propertyName);

            if (propertyName == ViewModel.GetPropertyName(p => p.BrowserURL))
            {
                LoadURL();
            }
        }

        protected override void OnInitializingActionSheet(List<string> titles)
        {
            if (ViewModel.OpenLinkCommand.CanExecute(null))
            {
                titles.Add(Localization.OpenInSafari);
            }

            titles.Add(Localization.Refresh);

            // TODO: To support Chrome some day
            //titles.Add(Localization.OpenInChrome);

            if (ViewModel.CopyLinkCommand.CanExecute(null))
            {
                titles.Add(Localization.CopyLink);
            }

            if (ViewModel.ShareCommand.CanExecute(null))
            {
                titles.Add(Localization.ShareButton);
            }
        }

        protected override void OnActionSheetClick(string buttonTitle)
        {
            switch (buttonTitle)
            {
                case Localization.OpenInSafari:
                    if (ViewModel.OpenLinkCommand.CanExecute(null))
                    {
                        ViewModel.OpenLinkCommand.Execute(null);
                    }
                    break;

                case Localization.Refresh:
                    WebView.Reload();
                    break;

                case Localization.CopyLink:
                    if (ViewModel.CopyLinkCommand.CanExecute(null))
                    {
                        ViewModel.CopyLinkCommand.Execute(null);
                    }
                    break;

                case Localization.ShareButton:
                    if (ViewModel.ShareCommand.CanExecute(null))
                    {
                        ViewModel.ShareCommand.Execute(null);
                    }
                    break;
            }
        }

        private void InitializeNavBarItems()
        {   
            var gap = ButtonBarUtil.CreateGapSpacer();

            _closeButton = ButtonBarUtil.Create(
                ThemeIcons.Close, 
                ThemeIcons.CloseLandscape);
            _closeButton.TouchUpInside += OnCloseButtonClick;
            var closeButtonItem = new UIBarButtonItem(_closeButton);

            _backButton = ButtonBarUtil.Create(
                ThemeIcons.Back, 
                ThemeIcons.BackLandscape);
            _backButton.TouchUpInside += OnBackButtonClick;
            _backButtonItem = new UIBarButtonItem(_backButton);

            NavigationItem.SetLeftBarButtonItems(new [] { gap, closeButtonItem, _backButtonItem }, true);

            _indicatorView = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.White) 
                {
                    Frame = new CGRect(0, 0, 40, 40)
                };
            _progressButtonItem = new UIBarButtonItem(_indicatorView);

            _forwardButton = ButtonBarUtil.Create(
                ThemeIcons.Forward, 
                ThemeIcons.ForwardLandscape);
            _forwardButton.TouchUpInside += OnForwardButtonClick;
            _forwardButtonItem = new UIBarButtonItem(_forwardButton);

            _moreButton = ButtonBarUtil.Create(
                ThemeIcons.More, 
                ThemeIcons.MoreLandscape);
            _moreButton.TouchUpInside += OnMoreButtonClicked;
            var moreButtonItem = new UIBarButtonItem(_moreButton);

            NavigationItem.SetRightBarButtonItems(
                new [] { gap, moreButtonItem, _forwardButtonItem, _progressButtonItem }, true);
        }

        private void DisposeNavBarItems()
        {
            if (_closeButton != null)
            {
                _closeButton.TouchUpInside -= OnCloseButtonClick;
            }

            if (_backButton != null)
            {
                _backButton.TouchUpInside -= OnBackButtonClick;
            }

            if (_forwardButton != null)
            {
                _forwardButton.TouchUpInside -= OnForwardButtonClick;
            }

            if (_moreButton != null)
            {
                _moreButton.TouchUpInside -= OnMoreButtonClicked;
            }
        }

        private void LoadURL()
        {
            if (ViewModel.BrowserURL != null)
            {
                var request = new NSUrlRequest(ViewModel.BrowserURL.ToNSUrl());
                WebView.LoadRequest(request);
            }
        }

        private void OnWebViewLoadStarted(object sender, EventArgs e)
        {
            _indicatorView.StartAnimating();

            UpdateNavButtonsState();
            UpdateViewTitle();
        }

        private void OnWebViewLoadFinished(object sender, EventArgs e)
        {
            _indicatorView.StopAnimating();

            UpdateNavButtonsState();
            UpdateViewTitle();
        }

        private void OnCloseButtonClick(object sender, EventArgs e)
        {
            if (ToHide != null)
            {
                ToHide(this, EventArgs.Empty);
            }
        }

        private void OnBackButtonClick(object sender, EventArgs e)
        {
            WebView.GoBack();
        }

        private void OnForwardButtonClick(object sender, EventArgs e)
        {
            WebView.GoForward();
        }

        private void OnMoreButtonClicked(object sender, EventArgs e)
        {
            ShowActionSheet();
        }

        private void UpdateNavButtonsState()
        {
            SetButtonEnabled(_backButtonItem, WebView.CanGoBack);
            SetButtonEnabled(_forwardButtonItem, WebView.CanGoForward);
            SetButtonEnabled(_progressButtonItem, _indicatorView.IsAnimating);
        }

        private static void SetButtonEnabled(UIBarButtonItem buttonItem, bool isEnabled)
        {
            buttonItem.Enabled = isEnabled;
            buttonItem.CustomView.Hidden = !isEnabled;

            var frame = buttonItem.CustomView.Frame;
            buttonItem.CustomView.Frame = isEnabled
                ? new CGRect(frame.Location, new CGSize(frame.Height, frame.Height))
                : new CGRect(frame.Location, new CGSize(0, frame.Height)); 
        }

        private void InitializeStyle()
        {
            WebView.BackgroundColor = UIColor.White;
        }

        private void DisposeWebView()
        {
            WebView.LoadStarted -= OnWebViewLoadStarted;
            WebView.LoadError -= OnWebViewLoadFinished;
            WebView.LoadFinished -= OnWebViewLoadFinished;
        }

        private void UpdateButtonsFrameOnRotation()
        {
            ButtonBarUtil.UpdateButtonsFrameOnRotation(NavigationItem.GetNavItemBarItems());
        }
    }
}