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
        private UIBarButtonItem _progressBarItem;

        public new BrowserViewModel ViewModel
        {
            get { return (BrowserViewModel)base.ViewModel; }
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            InitializeToolBar();

            ViewModel.PropertyChanged += OnViewModelPropertyChanged;

            WebView.LoadStarted += OnWebViewLoadStarted;
            WebView.LoadError += OnWebViewLoadFinished;
            WebView.LoadFinished += OnWebViewLoadFinished;

            LoadURL();
            UpdateNavButtonsState();
        }

        private void InitializeToolBar()
        {
            _indicatorView = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.White) {
                Frame = new RectangleF(0, 0, 40, 40)
            };
            _progressBarItem = new UIBarButtonItem(_indicatorView);
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
                var request = new NSUrlRequest(new NSUrl(ViewModel.BrowserURL));
                WebView.LoadRequest(request);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (_indicatorView != null)
            {
                _indicatorView.Dispose();
                _indicatorView = null;
            }

            if (_progressBarItem != null)
            {
                _progressBarItem.Dispose();
                _progressBarItem = null;
            }

            ReleaseDesignerOutlets();
            base.Dispose(disposing);
        }

        private void UpdateNavButtonsState()
        {
            BackButton.Enabled = WebView.CanGoBack;
            ForwardButton.Enabled = WebView.CanGoForward;
        }

        private void OnWebViewLoadStarted(object sender, EventArgs e)
        {
            _indicatorView.StartAnimating();

            if (NavigationItem.RightBarButtonItem != _progressBarItem)
            {
                NavigationItem.SetRightBarButtonItem(_progressBarItem, true);
            }

            UpdateNavButtonsState();
        }

        private void OnWebViewLoadFinished(object sender, EventArgs e)
        {
            _indicatorView.StopAnimating();
            NavigationItem.SetRightBarButtonItem(null, true);

            UpdateNavButtonsState();
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
    }
}