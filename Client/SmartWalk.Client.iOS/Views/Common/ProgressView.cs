using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Client.iOS.Utils;
using SmartWalk.Client.iOS.Resources;

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

        public static ProgressView Create()
        {
            return (ProgressView)Nib.Instantiate(null, null)[0];
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            if (NoDataLabel != null && !_isInitialized)
            {
                NoDataLabel.Text = Localization.NoContentAvailable;

                InitializeStyle();
                _isInitialized = true;
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            ConsoleUtil.LogDisposed(this);
        }

        private void InitializeStyle()
        {
            NoDataLabel.Font = Theme.NoDataFont;
            NoDataLabel.TextColor = Theme.LoadingText;

            ProgressLabel.Font = Theme.LoadingFont;
            ProgressLabel.TextColor = Theme.LoadingText;
        }
    }
}