using System;
using System.ComponentModel;
using System.Drawing;
using Cirrious.MvvmCross.Touch.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Core.Utils;
using SmartWalk.Core.ViewModels;

namespace SmartWalk.iOS.Views.Common
{
    public partial class BrowserView : MvxViewController
    {
        private UIActivityIndicatorView _indicatorView;

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
            if (ViewModel.BrowserURL != null)
            {
                var url = ViewModel.BrowserURL;
                if (!url.StartsWith(@"http://", true, null) &&
                    !url.StartsWith(@"https://", true, null))
                {
                    url = "http://" + url;
                }

                var request = new NSUrlRequest(new NSUrl(url));
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

        }

        private void UpdateNavButtonsState()
        {
            BackButton.Enabled = WebView.CanGoBack;
            ForwardButton.Enabled = WebView.CanGoForward;
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