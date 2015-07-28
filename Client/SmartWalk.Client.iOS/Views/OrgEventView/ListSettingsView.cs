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
        private static readonly UIColor Light = ThemeColors.ContentDarkText.ColorWithAlpha(0.95f);
        private static readonly UIColor Background = ThemeColors.HeaderBackground.GetDarker(0.3f);

        private bool _isInitialized;
        private bool _showOnlyFavotires;

        public const int DefaultHeight = 34;

        public ListSettingsView(IntPtr handle) : base(handle)
        {
        }

        public static ListSettingsView Create()
        {
            return (ListSettingsView)Nib.Instantiate(null, null)[0];
        }

        public ICommand SortByCommand { get; set; }
        public ICommand ShowOnlyFavoritesCommand { get; set; }

        public SortBy SortBy
        {
            get { return SortBySegments.SelectedSegment == 0 ? SortBy.Name : SortBy.Time; }
            set { SortBySegments.SelectedSegment = value == SortBy.Name ? 0 : 1; }
        }

        public bool ShowOnlyFavotires
        {
            get { return _showOnlyFavotires; }
            set 
            {
                if (_showOnlyFavotires != value)
                {
                    _showOnlyFavotires = value;
                    UpdateFavoritesState();
                }
            }
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
                SortBySegments.SetTitle(Localization.Name, 0);
                SortBySegments.SetTitle(Localization.Time, 1);

                SortByLabel.Text = Localization.OrderBy;
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
            BackgroundView.BarTintColor = Background;
            Separator.Color = ThemeColors.HeaderBackground.GetDarker(0.4f);

            SortByLabel.Font = Theme.OrgEventHeaderFont;
            SortByLabel.TextColor = Light;

            SortBySegments.SetTitleTextAttributes(
                new UITextAttributes {
                    Font = Theme.OrgEventHeaderFont, 
                    TextColor = Light
                }, 
                UIControlState.Highlighted);
            SortBySegments.SetTitleTextAttributes(
                new UITextAttributes {
                    Font = Theme.OrgEventHeaderFont,
                    TextColor = Light
                }, 
                UIControlState.Normal);
            SortBySegments.TintColor = Light;

            FavoritesButton.Font = Theme.OrgEventHeaderFont;
            FavoritesButton.SetTitleColor(Light, UIControlState.Normal);
            FavoritesButton.SetImage(ThemeIcons.StarSmall, UIControlState.Normal);
            FavoritesButton.TintColor = Light;
            FavoritesButton.Layer.BorderWidth = 0.9f;
            FavoritesButton.Layer.CornerRadius = 4;
            FavoritesButton.Layer.BorderColor = Light.CGColor;

            FavoritesButton.TouchDown += (sender, e) => 
                FavoritesButton.BackgroundColor = ThemeColors.ContentLightBackground.ColorWithAlpha(0.2f);
            FavoritesButton.TouchUpInside += (sender, e) => UpdateFavoritesState();
            FavoritesButton.TouchUpOutside += (sender, e) => UpdateFavoritesState();
        }

        partial void OnFavoritesClick(UIButton sender)
        {
            ShowOnlyFavotires = !ShowOnlyFavotires;

            if (ShowOnlyFavoritesCommand.CanExecute(ShowOnlyFavotires))
            {
                ShowOnlyFavoritesCommand.Execute(ShowOnlyFavotires);
            }
        }

        private void UpdateFavoritesState()
        {
            UIView.Animate(UIConstants.AnimationDuration,
                () =>
                {
                    if (ShowOnlyFavotires)
                    {
                        FavoritesButton.SetTitleColor(Background, UIControlState.Normal);
                        FavoritesButton.TintColor = Background;
                        FavoritesButton.BackgroundColor = Light;
                    }
                    else
                    {
                        FavoritesButton.SetTitleColor(Light, UIControlState.Normal);
                        FavoritesButton.TintColor = Light;
                        FavoritesButton.BackgroundColor = UIColor.Clear;
                    }
                });
        }
    }
}