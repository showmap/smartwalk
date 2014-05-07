using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Client.Core.ViewModels;
using SmartWalk.Shared.Utils;
using SmartWalk.Client.iOS.Resources;
using SmartWalk.Client.iOS.Utils;
using SmartWalk.Client.iOS.Views.Common.Base;

namespace SmartWalk.Client.iOS.Views.Common
{
    public partial class BrowserView : CustomNavBarViewBase
    {
        private const string DocTitle = "document.title";
        private const string Http = "http://";
        private const string Https = "https://";

        private UIActivityIndicatorView _indicatorView;

        private string BrowserURL
        {
            get
            {
                if (ViewModel.BrowserURL != null)
                {
                    var url = ViewModel.BrowserURL;
                    if (!url.StartsWith(Http, StringComparison.OrdinalIgnoreCase) &&
                        !url.StartsWith(Https, StringComparison.OrdinalIgnoreCase))
                    {
                        url = Http + url;
                    }

                    return url;
                }

                return null;
            }
        }

        public new BrowserViewModel ViewModel
        {
            get { return (BrowserViewModel)base.ViewModel; }
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

            LoadURL();
            UpdateNavButtonsState();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            UpdateFrames();
            ButtonBarUtil.UpdateButtonsFrameOnRotation(BottomToolbar.Items);
        }

        public override void WillAnimateRotation(UIInterfaceOrientation toInterfaceOrientation, double duration)
        {
            base.WillAnimateRotation(toInterfaceOrientation, duration);

            UpdateFrames();
            ButtonBarUtil.UpdateButtonsFrameOnRotation(BottomToolbar.Items);
        }

        protected override void OnViewModelPropertyChanged(string propertyName)
        {
            base.OnViewModelPropertyChanged(propertyName);

            if (propertyName == ViewModel.GetPropertyName(p => p.BrowserURL))
            {
                LoadURL();
            }
        }

        protected override void OnInitializingActionSheet(UIActionSheet actionSheet)
        {
            using (var url = new NSUrl(BrowserURL))
            {
                if (UIApplication.SharedApplication.CanOpenUrl(url))
                {
                    actionSheet.AddButton(Localization.OpenInSafari);
                }
            }

            // TODO: To support Chrome some day
            //actionSheet.AddButton(Localization.OpenInChrome);

            actionSheet.AddButton(Localization.CopyLink);
        }

        // TODO: Move to ViewModel's commands
        protected override void OnActionSheetClick(string buttonTitle)
        {
            switch (buttonTitle)
            {
                case Localization.OpenInSafari:
                    using (var url = new NSUrl(BrowserURL))
                    {
                        if (UIApplication.SharedApplication.CanOpenUrl(url))
                        {
                            UIApplication.SharedApplication.OpenUrl(url);
                        }
                    }
                    break;

                case Localization.CopyLink:
                    UIPasteboard.General.String = BrowserURL;
                    break;
            }
        }

        private void InitializeIndicator()
        {
            _indicatorView = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.White) {
                Frame = new RectangleF(0, 0, 40, 40)
            };
            ProgressButton.CustomView = _indicatorView;
        }

        private void LoadURL()
        {
            if (BrowserURL != null)
            {
                var request = new NSUrlRequest(new NSUrl(BrowserURL));
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
            SetButtonEnabled(ActionButton, BrowserURL != null);
        }

        private static void SetButtonEnabled(UIBarButtonItem buttonItem, bool isEnabled)
        {
            buttonItem.Enabled = isEnabled;
            buttonItem.CustomView.Alpha = isEnabled ? 1f : 0.5f;
        }

        private void UpdateViewTitle()
        {
            var pageTitle = WebView.EvaluateJavascript(DocTitle);
            NavigationItem.Title = !string.IsNullOrEmpty(pageTitle)
                ? pageTitle
                : (WebView.CanGoBack ? string.Empty : ViewModel.BrowserURL);
        }

        private void InitializeStyle()
        {
            if (UIDevice.CurrentDevice.CheckSystemVersion(7, 0))
            {
                BottomToolbar.BarTintColor = Theme.NavBarBackgroundiOS7;
            }
            else
            {
                BottomToolbar.SetBackgroundImage(
                    Theme.NavBarBackgroundImage,
                    UIToolbarPosition.Any,
                    UIBarMetrics.Default);
                BottomToolbar.SetBackgroundImage(
                    Theme.NavBarLandscapeBackgroundImage,
                    UIToolbarPosition.Any,
                    UIBarMetrics.LandscapePhone);
            }

            LeftSpacer.Width = Theme.ToolBarPaddingCompensate;

            var button = ButtonBarUtil.Create(ThemeIcons.BrowserBack, ThemeIcons.BrowserBackLandscape);
            button.TouchUpInside += OnBackButtonClick;
            BackButton.CustomView = button;

            button = ButtonBarUtil.Create(ThemeIcons.BrowserForward, ThemeIcons.BrowserForwardLandscape);
            button.TouchUpInside += OnForwardButtonClick;
            ForwardButton.CustomView = button;

            button = ButtonBarUtil.Create(ThemeIcons.BrowserRefresh, ThemeIcons.BrowserRefreshLandscape);
            button.TouchUpInside += OnRefreshButtonClick;
            RefreshButton.CustomView = button;

            button = ButtonBarUtil.Create(ThemeIcons.BrowserMenu, ThemeIcons.BrowserMenuLandscape);
            button.TouchUpInside += OnActionButtonClick;
            ActionButton.CustomView = button;

            RightSpacer.Width = Theme.ToolBarPaddingCompensate;
        }

        private void OnActionButtonClick(object sender, EventArgs e)
        {
            ShowActionSheet();
        }

        // HACK: This is to keep bottom toolbar proper height on rotation
        // by default it doesn't work with autolayout :-(
        private void UpdateFrames()
        {
            var topBarFrame = NavigationController.Toolbar.Frame;
            BottomToolbar.Frame = new RectangleF(
                0, 
                View.Frame.Height - topBarFrame.Height, 
                View.Frame.Width,
                topBarFrame.Height);

            var webFrame = WebView.Frame;
            webFrame.Height = View.Bounds.Height - topBarFrame.Height;
            WebView.Frame = webFrame;
        }
    }
}