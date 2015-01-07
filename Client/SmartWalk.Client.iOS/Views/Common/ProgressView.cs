using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
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

        public bool IsDataUnavailable
        {
            get { return !NoDataLabel.Hidden; }
            set { NoDataLabel.Hidden = !value; }
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
            if (NoDataLabel != null && 
                ProgressLabel != null &&
                !_isInitialized)
            {
                NoDataLabel.Text = Localization.NoContentAvailable;
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
            NoDataLabel.Font = Theme.ContentFont;
            NoDataLabel.TextColor = ThemeColors.ContentLightTextPassive;

            ProgressLabel.Font = Theme.ContentFont;
            ProgressLabel.TextColor = ThemeColors.ContentLightTextPassive;
        }
    }
}