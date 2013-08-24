using System;
using System.ComponentModel;
using System.Drawing;
using Cirrious.MvvmCross.Touch.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Core.Utils;
using SmartWalk.Core.ViewModels;
using SmartWalk.iOS.Controls;

namespace SmartWalk.iOS.Views.Common
{
    public partial class BrowserView : MvxViewController
    {
        private UIActivityIndicatorView _indicatorView;

        private string BrowserURL
        {
            get
            {
                if (ViewModel.BrowserURL != null)
                {
                    var url = ViewModel.BrowserURL;
                    if (!url.StartsWith(@"http://", true, null) &&
                        !url.StartsWith(@"https://", true, null))
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

            InitializeIndicator();
            UpdateViewTitle();

            ViewModel.PropertyChanged += OnViewModelPropertyChanged;

            WebView.LoadStarted += OnWebViewLoadStarted;
            WebView.LoadError += OnWebViewLoadFinished;
            WebView.LoadFinished += OnWebViewLoadFinished;

            LoadURL();
            UpdateNavButtonsState();
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

        partial void OnBackButtonClick(NSObject sender)
        {
            WebView.GoBack();
        }

        partial void OnForwardButtonClick(NSObject sender)
        {
            WebView.GoForward();
        }

        partial void OnRefreshButtonClick(NSObject sender)
        {
            WebView.Reload();
        }

        partial void OnActionButtonClick(NSObject sender)
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
            BackButton.Enabled = WebView.CanGoBack;
            ForwardButton.Enabled = WebView.CanGoForward;
            ActionButton.Enabled = BrowserURL != null;
        }

        private void UpdateViewTitle()
        {
            var pageTitle = WebView.EvaluateJavascript("document.title");
            NavigationItem.Title = pageTitle != null && pageTitle != string.Empty
                ? pageTitle
                    : (WebView.CanGoBack ? null: ViewModel.BrowserURL);
        }
    }
}