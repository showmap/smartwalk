using System;
using System.Windows.Input;
using Foundation;
using SmartWalk.Client.Core.Resources;
using SmartWalk.Client.Core.ViewModels;
using SmartWalk.Client.iOS.Resources;
using SmartWalk.Client.iOS.Utils;
using UIKit;

namespace SmartWalk.Client.iOS.Views.OrgEventView
{
    public partial class ListSettingsView : UIView
    {
        public static readonly UINib Nib = UINib.FromName("ListSettingsView", NSBundle.MainBundle);

        private bool _isInitialized;

        public const int DefaultHeight = 34;

        public ListSettingsView(IntPtr handle) : base(handle)
        {
        }

        public static ListSettingsView Create()
        {
            return (ListSettingsView)Nib.Instantiate(null, null)[0];
        }

        public ICommand SortByCommand { get; set; }

        public SortBy SortBy
        {
            get { return SortBySegments.SelectedSegment == 0 ? SortBy.Name : SortBy.Time; }
            set { SortBySegments.SelectedSegment = value == SortBy.Name ? 0 : 1; }
        }

        public override void WillMoveToSuperview(UIView newsuper)
        {
            base.WillMoveToSuperview(newsuper);

            if (newsuper == null)
            {
                SortByCommand = null;
            }
        }

        public override void LayoutSubviews()
        {
            if (SortByLabel != null &&
                !_isInitialized)
            {
                SortByLabel.Text = Localization.SortBy;
                // HACK: A space for a gap between start and caption
                FavoritesButton.SetTitle(" " + Localization.Favorites, UIControlState.Normal);

                InitializeStyle();
                _isInitialized = true;
            }

            base.LayoutSubviews();
        }

        partial void OnSortBySegmentsValueChanged(NSObject sender)
        {
            if (SortByCommand != null &&
                SortByCommand.CanExecute(SortBy))
            {
                SortByCommand.Execute(SortBy);
            }
        }

        private void InitializeStyle()
        {
            BackgroundView.BarTintColor = ThemeColors.HeaderBackground.GetDarker(0.3f);
            Separator.Color = ThemeColors.HeaderBackground.GetDarker(0.4f);

            var color = ThemeColors.ContentDarkText.ColorWithAlpha(0.95f);

            SortByLabel.Font = Theme.OrgEventHeaderFont;
            SortByLabel.TextColor = color;

            SortBySegments.SetTitleTextAttributes(
                new UITextAttributes {
                    Font = Theme.SegmentsTextFont, 
                    TextColor = color
                }, 
                UIControlState.Highlighted);
            SortBySegments.SetTitleTextAttributes(
                new UITextAttributes {
                    Font = Theme.SegmentsTextFont,
                    TextColor = color
                }, 
                UIControlState.Normal);
            SortBySegments.TintColor = color;

            FavoritesButton.Font = Theme.OrgEventHeaderFont;
            FavoritesButton.SetTitleColor(color, UIControlState.Normal);
            FavoritesButton.SetImage(ThemeIcons.StarSmall, UIControlState.Normal);
            FavoritesButton.TintColor = color;
            FavoritesButton.Layer.BorderWidth = 0.9f;
            FavoritesButton.Layer.CornerRadius = 4;
            FavoritesButton.Layer.BorderColor = color.CGColor;

            FavoritesButton.TouchDown += (sender, e) => 
                FavoritesButton.BackgroundColor = ThemeColors.ContentDarkText.ColorWithAlpha(0.2f);
            FavoritesButton.TouchUpInside += (sender, e) => FavoritesButton.BackgroundColor = UIColor.Clear;
            FavoritesButton.TouchUpOutside += (sender, e) => FavoritesButton.BackgroundColor = UIColor.Clear;
        }

        partial void OnFavoritesClick(UIButton sender)
        {
            
        }
    }
}