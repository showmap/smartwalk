using System;
using Foundation;
using UIKit;
using SmartWalk.Client.Core.Resources;
using SmartWalk.Client.iOS.Resources;
using SmartWalk.Client.iOS.Utils;

namespace SmartWalk.Client.iOS.Views.Common
{
    public partial class ProgressView : UIView
    {
        public static readonly UINib Nib = UINib.FromName("ProgressView", NSBundle.MainBundle);

        private bool _isInitialized;

        public ProgressView(IntPtr handle) : base(handle)
        {
        }

        public bool IsLoading
        {
            get { return !LoadingView.Hidden; }
            set { LoadingView.Hidden = !value; }
        }

        public bool IsMessageVisible
        {
            get { return !MessageLabel.Hidden; }
            set { MessageLabel.Hidden = !value; }
        }

        public string MessageText
        {
            get { return MessageLabel.Text; }
            set { MessageLabel.Text = value; }
        }

        public UIActivityIndicatorViewStyle IndicatorStyle
        {
            get { return ActivityIndicator.ActivityIndicatorViewStyle; }
            set { ActivityIndicator.ActivityIndicatorViewStyle = value; }
        }

        public static ProgressView Create()
        {
            return (ProgressView)Nib.Instantiate(null, null)[0];
        }

        public override void LayoutSubviews()
        {
            if (MessageLabel != null && 
                ProgressLabel != null &&
                !_isInitialized)
            {
                MessageLabel.Text = Localization.NoContentAvailable;
                ProgressLabel.Text = Localization.Loading + "...";

                InitializeStyle();
                _isInitialized = true;
            }

            base.LayoutSubviews();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            ConsoleUtil.LogDisposed(this);
        }

        private void InitializeStyle()
        {
            MessageLabel.Font = Theme.ContentFont;
            MessageLabel.TextColor = ThemeColors.ContentLightTextPassive;

            ProgressLabel.Font = Theme.ContentFont;
            ProgressLabel.TextColor = ThemeColors.ContentLightTextPassive;
        }
    }
}