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

        private UIActivityIndicatorView _indicatorView;

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

            BottomToolbar.Victim = WebView;
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            UpdateViewConstraints();
            ButtonBarUtil.UpdateButtonsFrameOnRotation(BottomToolbar.Items);
        }

        public override void WillAnimateRotation(UIInterfaceOrientation toInterfaceOrientation, double duration)
        {
            base.WillAnimateRotation(toInterfaceOrientation, duration);

            UpdateViewConstraints();
            ButtonBarUtil.UpdateButtonsFrameOnRotation(BottomToolbar.Items);
        }

        public override void UpdateViewConstraints()
        {
            base.UpdateViewConstraints();

            ToolBarHeightConstraint.Constant =
                ScreenUtil.IsVerticalOrientation ? 44 : 33;
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
            if (ViewModel.OpenLinkCommand.CanExecute(null))
            {
                actionSheet.AddButton(Localization.OpenInSafari);
            }

            // TODO: To support Chrome some day
            //actionSheet.AddButton(Localization.OpenInChrome);

            if (ViewModel.CopyLinkCommand.CanExecute(null))
            {
                actionSheet.AddButton(Localization.CopyLink);
            }

            if (ViewModel.ShareCommand.CanExecute(null))
            {
                actionSheet.AddButton(Localization.ShareButton);
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
                Frame = new RectangleF(0, 0, 40, 40)
            };
            ProgressButton.CustomView = _indicatorView;
        }

        private void LoadURL()
        {
            if (ViewModel.BrowserURL != null)
            {
                var request = new NSUrlRequest(new NSUrl(ViewModel.BrowserURL));
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

        private void UpdateViewTitle()
        {
            var pageTitle = WebView.EvaluateJavascript(DocTitle);
            NavigationItem.Title = !string.IsNullOrEmpty(pageTitle)
                ? pageTitle
                : (WebView.CanGoBack ? string.Empty : ViewModel.BrowserURL);
        }

        private void InitializeStyle()
        {
            LeftSpacer.Width = 
                UIDevice.CurrentDevice.CheckSystemVersion(7, 0)
                    ? Theme.NavBarPaddingCompensate
                    : Theme.CustomNavBarPaddingCompensate;

            var button = ButtonBarUtil.Create(ThemeIcons.BrowserBack, ThemeIcons.BrowserBackLandscape, true);
            button.TouchUpInside += OnBackButtonClick;
            BackButton.CustomView = button;

            button = ButtonBarUtil.Create(ThemeIcons.BrowserForward, ThemeIcons.BrowserForwardLandscape, true);
            button.TouchUpInside += OnForwardButtonClick;
            ForwardButton.CustomView = button;

            button = ButtonBarUtil.Create(ThemeIcons.BrowserRefresh, ThemeIcons.BrowserRefreshLandscape, true);
            button.TouchUpInside += OnRefreshButtonClick;
            RefreshButton.CustomView = button;
        }
    }
}