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
    public partial class BrowserView : NavBarViewBase
    {
        private const string DocTitle = "document.title";

        private bool _toolbarsHidden;
        private UIActivityIndicatorView _indicatorView;

        public new BrowserViewModel ViewModel
        {
            get { return (BrowserViewModel)base.ViewModel; }
        }

        public bool ToolbarsHidden
        {
            get
            {
                return _toolbarsHidden;
            }
            set
            {
                if (_toolbarsHidden != value)
                {
                    _toolbarsHidden = value;

                    if (_toolbarsHidden)
                    {
                        SetNavBarHidden(true, true);
                        BottomToolbar.SetHidden(true, true);
                    }
                    else
                    {
                        SetNavBarHidden(false, true);
                        BottomToolbar.SetHidden(false, true);
                    }
                }
            }
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

            InitializeStyle();
            InitializeIndicator();
            UpdateViewTitle();

            WebView.LoadStarted += OnWebViewLoadStarted;
            WebView.LoadError += OnWebViewLoadFinished;
            WebView.LoadFinished += OnWebViewLoadFinished;

            WebView.ScrollView.Delegate = new BrowserScrollViewDelegate(this, WebView);

            LoadURL();
            UpdateNavButtonsState();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            UpdateViewConstraints();
            ButtonBarUtil.UpdateButtonsFrameOnRotation(BottomToolbar.Items);
        }

        public override void WillAnimateRotation(
            UIInterfaceOrientation toInterfaceOrientation, 
            double duration)
        {
            base.WillAnimateRotation(toInterfaceOrientation, duration);

            UpdateViewConstraints();
            ButtonBarUtil.UpdateButtonsFrameOnRotation(BottomToolbar.Items);
        }

        public override void UpdateViewConstraints()
        {
            base.UpdateViewConstraints();

            ToolBarHeightConstraint.Constant =
                ScreenUtil.IsVerticalOrientation 
                    ? UIConstants.ToolBarVerticalHeight 
                    : UIConstants.ToolBarHorizontalHeight;
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

        private void InitializeIndicator()
        {
            _indicatorView = new UIActivityIndicatorView(
                UIActivityIndicatorViewStyle.Gray) 
            {
                Frame = new CGRect(0, 0, 40, 40)
            };
            ProgressButton.CustomView = _indicatorView;
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
            _indicatorView.Hidden = false;

            UpdateNavButtonsState();
            UpdateViewTitle();
        }

        private void OnWebViewLoadFinished(object sender, EventArgs e)
        {
            _indicatorView.StopAnimating();
            _indicatorView.Hidden = true;

            UpdateNavButtonsState();
            UpdateViewTitle();
        }

        private void OnBackButtonClick(object sender, EventArgs e)
        {
            WebView.GoBack();
        }

        private void OnForwardButtonClick(object sender, EventArgs e)
        {
            WebView.GoForward();
        }

        private void OnRefreshButtonClick(object sender, EventArgs e)
        {
            WebView.Reload();
        }

        private void UpdateNavButtonsState()
        {
            SetButtonEnabled(BackButton, WebView.CanGoBack);
            SetButtonEnabled(ForwardButton, WebView.CanGoForward);
        }

        private static void SetButtonEnabled(UIBarButtonItem buttonItem, bool isEnabled)
        {
            buttonItem.Enabled = isEnabled;
            buttonItem.CustomView.Alpha = isEnabled ? 1f : 0.5f;
        }

        private void InitializeStyle()
        {
            WebView.BackgroundColor = UIColor.White;
            LeftSpacer.Width = Theme.NavBarPaddingCompensate;

            BottomToolbar.Translucent = true;
            BottomToolbar.SetBackgroundImage(new UIImage(), UIToolbarPosition.Any, UIBarMetrics.Default);
            BottomToolbar.SetShadowImage(new UIImage(), UIToolbarPosition.Any);
            BottomToolbar.BarTintColor = UIColor.Clear;

            var button = ButtonBarUtil.Create(
                ThemeIcons.Back, 
                ThemeIcons.BackLandscape, 
                SemiTransparentType.Light);
            button.TouchUpInside += OnBackButtonClick;
            BackButton.CustomView = button;

            button = ButtonBarUtil.Create(
                ThemeIcons.Forward, 
                ThemeIcons.ForwardLandscape, 
                SemiTransparentType.Light);
            button.TouchUpInside += OnForwardButtonClick;
            ForwardButton.CustomView = button;

            button = ButtonBarUtil.Create(
                ThemeIcons.Refresh, 
                ThemeIcons.RefreshLandscape, 
                SemiTransparentType.Light);
            button.TouchUpInside += OnRefreshButtonClick;
            RefreshButton.CustomView = button;
        }
    }

    public class BrowserScrollViewDelegate : UIScrollViewDelegate
    {
        private readonly UIWebView _webView;
        private readonly ScrollToHideUIManager _scrollToHideManager;

        public BrowserScrollViewDelegate(BrowserView view, UIWebView webView)
        {
            _webView = webView;
            _scrollToHideManager = new BrowserScrollToHideUIManager(view, _webView.ScrollView);
        }

        public override void DraggingStarted(UIScrollView scrollView)
        {
            _scrollToHideManager.DraggingStarted();
        }

        public override void DraggingEnded(UIScrollView scrollView, bool willDecelerate)
        {
            _scrollToHideManager.DraggingEnded();
        }

        public override void Scrolled(UIScrollView scrollView)
        {
            _scrollToHideManager.Scrolled();
        }

        public override void ScrolledToTop(UIScrollView scrollView)
        {
            _scrollToHideManager.ScrolledToTop();
        }
    }

    public class BrowserScrollToHideUIManager : ScrollToHideUIManager
    {
        private readonly BrowserView _view;

        public BrowserScrollToHideUIManager(BrowserView view, UIScrollView scrollView) 
            : base(scrollView)
        {
            _view = view;
        }

        protected override void OnHideUI()
        {
            _view.ToolbarsHidden = true;
        }

        protected override void OnShowUI()
        {
            _view.ToolbarsHidden = false;
        }
    }
}