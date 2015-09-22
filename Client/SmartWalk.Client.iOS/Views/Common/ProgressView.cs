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

        public ProgressView(IntPtr handle) : base(handle)
        {
            ContentView = (UIView)Nib.Instantiate(this, null)[0];
            ContentView.Frame = Bounds;
            ContentView.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;
            Add(ContentView);
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

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            MessageLabel.Text = Localization.NoContentAvailable;
            ProgressLabel.Text = Localization.Loading + "...";

            InitializeStyle();
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