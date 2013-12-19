using System;
using System.ComponentModel;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Client.Core.Utils;
using SmartWalk.Client.Core.ViewModels;
using SmartWalk.Client.iOS.Utils;
using SmartWalk.Client.iOS.Resources;
using System.Globalization;

namespace SmartWalk.Client.iOS.Views.Common
{
    public partial class BrowserView : ActiveAwareViewController
    {
        private UIActivityIndicatorView _indicatorView;

        private string BrowserURL
        {
            get
            {
                if (ViewModel.BrowserURL != null)
                {
                    var url = ViewModel.BrowserURL;
                    if (!url.StartsWith(@"http://", true, CultureInfo.InvariantCulture) &&
                        !url.StartsWith(@"https://", true, CultureInfo.InvariantCulture))
                    {
                        url = "http://" + url;
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

            ButtonBarUtil.OverrideNavigatorBackButton(
                NavigationItem,
                () => NavigationController.PopViewControllerAnimated(true));

            InitializeStyle();
            InitializeIndicator();
            UpdateViewTitle();

            ViewModel.PropertyChanged += OnViewModelPropertyChanged;

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
            ButtonBarUtil.UpdateButtonsFrameOnRotation(NavigationItem.LeftBarButtonItems);
            ButtonBarUtil.UpdateButtonsFrameOnRotation(BottomToolbar.Items);
        }

        public override void WillAnimateRotation(UIInterfaceOrientation toInterfaceOrientation, double duration)
        {
            base.WillAnimateRotation(toInterfaceOrientation, duration);

            UpdateFrames();
            ButtonBarUtil.UpdateButtonsFrameOnRotation(NavigationItem.LeftBarButtonItems);
            ButtonBarUtil.UpdateButtonsFrameOnRotation(BottomToolbar.Items);
        }

        private void InitializeIndicator()
        {
            _indicatorView = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.White) {
                Frame = new RectangleF(0, 0, 40, 40)
            };
            ProgressButton.CustomView = _indicatorView;
        }

        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == ViewModel.GetPropertyName(p => p.BrowserURL))
            {
                LoadURL();
            }
        }

        private void LoadURL()
        {
            if (BrowserURL != null)
            {
                var request = new NSUrlRequest(new NSUrl(BrowserURL));
                WebView.LoadRequest(request);
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            ConsoleUtil.LogDisposed(this);
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

        private void OnActionButtonClick(object sender, EventArgs e)
        {
            var actionSheet = new UIActionSheet();
            actionSheet.Style = UIActionSheetStyle.BlackTranslucent;
            actionSheet.Clicked += OnActionClicked;

            // TODO: Localize
            using (var url = new NSUrl(BrowserURL))
            {
                if (UIApplication.SharedApplication.CanOpenUrl(url))
                {
                    actionSheet.AddButton("Open In Safari");
                }
            }

            // TODO: To support Chrome some day
            //actionSheet.AddButton("Open In Chrome");

            actionSheet.AddButton("Copy Link");
            actionSheet.AddButton("Cancel");

            actionSheet.CancelButtonIndex = actionSheet.ButtonCount - 1;

            actionSheet.ShowInView(View);
        }

        private void OnActionClicked(object sender, UIButtonEventArgs e)
        {
            var actionSheet = ((UIActionSheet)sender);
            actionSheet.Clicked -= OnActionClicked;

            switch (actionSheet.ButtonCount - e.ButtonIndex)
            {
                case 3:
                    using (var url = new NSUrl(BrowserURL))
                    {
                        if (UIApplication.SharedApplication.CanOpenUrl(url))
                        {
                            UIApplication.SharedApplication.OpenUrl(url);
                        }
                    }
                    break;

                case 2:
                    UIPasteboard.General.String = BrowserURL;
                    break;
            }
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
            var pageTitle = WebView.EvaluateJavascript("document.title");
            NavigationItem.Title = !string.IsNullOrEmpty(pageTitle)
                ? pageTitle
                : (WebView.CanGoBack ? null : ViewModel.BrowserURL);
        }

        private void InitializeStyle()
        {
            LeftSpacer.Width = -12;

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

            RightSpacer.Width = -12;
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