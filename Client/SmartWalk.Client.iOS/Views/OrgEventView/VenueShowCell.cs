using System;
using System.Linq;
using System.Windows.Input;
using Cirrious.MvvmCross.Binding.Touch.Views;
using CoreGraphics;
using Foundation;
using SmartWalk.Client.Core.Model;
using SmartWalk.Client.Core.Model.DataContracts;
using SmartWalk.Client.Core.Resources;
using SmartWalk.Client.Core.Utils;
using SmartWalk.Client.Core.ViewModels.Interfaces;
using SmartWalk.Client.iOS.Resources;
using SmartWalk.Client.iOS.Utils;
using SmartWalk.Client.iOS.Utils.MvvmCross;
using SmartWalk.Client.iOS.Views.Common.Base.Cells;
using SmartWalk.Shared.Utils;
using UIKit;

namespace SmartWalk.Client.iOS.Views.OrgEventView
{
    public partial class VenueShowCell : TableCellBase
    {
        private static readonly NSDateFormatter TimeFormatter = new NSDateFormatter {
            DateStyle = NSDateFormatterStyle.None,
            TimeStyle = NSDateFormatterStyle.Short,
            TimeZone = NSTimeZone.FromAbbreviation("UTC")
        };

        private const string Space = " ";
        private const char M = 'm';

        private const float HorizontalBorder = 10f;
        private const float VerticalBorder = 15f;
        private const float ImageSmallHeight = 40f;
        private const float ImageLargeHeight = 120f;
        private const float HorizontalGap = 5f;
        private const float VerticalGap = 12f;
        private const float TextGap = 5f;
        private const float TimeBorderGap = 44f;
        private const float DetailsTapAreaHeight = 50f;

        private static readonly float ShowTitleTextHeight = 
            ScreenUtil.CalculateTextHeight(300, "Showp", Theme.ContentFont);
        private static readonly float ShowLocationTextHeight = 
            ScreenUtil.CalculateTextHeight(300, "Showp", Theme.VenueShowDescriptionFont);
        private static readonly float DetailsTextHeight = 
            ScreenUtil.CalculateTextHeight(300, Localization.MoreInformation, Theme.VenueShowDetailsFont);

        public static readonly UINib Nib = UINib.FromName("VenueShowCell", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("VenueShowCell");

        public static readonly float DefaultHeight = VerticalBorder + ShowTitleTextHeight + VerticalBorder;

        private readonly MvxResizedImageViewLoader _smallImageHelper;
        private readonly MvxImageViewLoader _largeImageHelper;
        private readonly AnimationDelay _smallImageAnimationDelay = new AnimationDelay();
        private readonly AnimationDelay _largeImageAnimationDelay = new AnimationDelay();

        private CGSize _lastLayoutSize;
        private UITapGestureRecognizer _cellTapGesture;
        private UITapGestureRecognizer _starTapGesture;
        private bool _isBeforeExpanded;
        private bool _isExpanded;

        public VenueShowCell(IntPtr handle) : base(handle)
        {
            BackgroundView = new UIView { BackgroundColor = ThemeColors.ContentLightBackground };

            _smallImageHelper = new MvxResizedImageViewLoader(
                () => ThumbImageView,
                () => ThumbImageView.Bounds,
                state => 
                {
                    if (_smallImageHelper.ImageUrl != null && 
                        ThumbImageView.ProgressEnded(state))
                    {
                        if (ThumbImageView.HasImage(state))
                        {
                            ThumbImageView.SetHidden(false, _smallImageAnimationDelay.Animate);
                        }
                        else 
                        {
                            // showing abbr if image couldn't be loaded
                            ThumbImageView.Image = ThemeIcons.Circle;
                            ThumbLabel.SetHidden(false, false);
                        }
                    }
                }) { UseRoundClip = true };

            _largeImageHelper = new MvxImageViewLoader(
                () => ThumbImageView, 
                () => 
                {
                    if (_largeImageHelper.ImageUrl != null && 
                        ThumbImageView.HasImage())
                    {
                        ThumbImageView.SetHidden(false, _largeImageAnimationDelay.Animate);
                        UpdateConstraintConstants(false);
                    }
                });
            _largeImageHelper.DefaultImagePath = Theme.DefaultImagePath;
            _largeImageHelper.ErrorImagePath = Theme.ErrorImagePath;
        }

        public static float CalculateCellHeight(nfloat frameWidth, bool isExpanded,
            Show show, bool showTime = true, bool includeLocation = false)
        {
            if (isExpanded)
            {
                var cellHeight = 0f;
                var textHeight = GetTextBlocksHeight(frameWidth, show, showTime, includeLocation);

                var imageHeight = show.HasPictures() ? VerticalGap + ImageLargeHeight : 0f;
                var detailsHeight = show.HasDetailsUrl() ? VerticalGap + DetailsTextHeight : 0f;

                // if no text, we still show times
                cellHeight += Math.Max(DefaultHeight, textHeight + imageHeight + detailsHeight + VerticalBorder); 

                return cellHeight;
            }

            return DefaultHeight;
        }

        private static float GetTextBlocksHeight(nfloat frameWidth, Show show, 
            bool showTime = true, bool includeLocation = false)
        {
            if (show == null) return 0;

            var titleHeight = 
                !string.IsNullOrWhiteSpace(show.Title)
                ? ScreenUtil.CalculateTextHeight(
                    GetTitleBlockWidth(frameWidth, show, showTime), 
                    show.Title, 
                    Theme.ContentFont)
                : 0f;

            var locationHeight = includeLocation
                ? (titleHeight > 0 ? TextGap : 0f) + ShowLocationTextHeight
                : 0f;

            var showDescription = GetShowDescription(show, !showTime);
            var descriptionHeight = 
                !string.IsNullOrWhiteSpace(showDescription)
                ? (titleHeight > 0 ? TextGap : 0f) + ScreenUtil.CalculateTextHeight(
                    GetDescriptionBlockWidth(frameWidth), 
                    showDescription, 
                    Theme.VenueShowDescriptionFont)
                : 0f;

            var textHeight = VerticalBorder + titleHeight + locationHeight + descriptionHeight;
            return textHeight;
        }

        private static float GetTitleBlockWidth(nfloat frameWidth, Show show, bool showTime = true)
        {
            // - Left Border Gap - Time Block Width (inc. Right Border Gap)
            return (float)(frameWidth - HorizontalBorder - GetTimeBlockWidth(showTime ? show : null));
        }

        private static float GetDescriptionBlockWidth(nfloat frameWidth)
        {
            // - Left Border Gap - Right Border Gap
            return (float)(frameWidth - HorizontalBorder - HorizontalBorder);
        }

        private static float GetTimeBlockWidth(Show show)
        {
            var result = HorizontalGap + TimeBorderGap; // Title Label Gap + Time Label Right Gap

            if (show != null)
            {
                var time = default(string);
                var font = default(UIFont);

                if (show.StartTime.HasValue)
                {
                    time = GetShowTimeText(show.StartTime);
                    font = GetShowStartTimeFont(show.Status);

                }
                else if (show.EndTime.HasValue)
                {
                    time = GetShowTimeText(show.EndTime);
                    font = GetShowEndTimeFont(show.Status);
                }

                var width = time != null 
                    ? ScreenUtil.CalculateTextWidth(20, time, font) : 0f;
                result += width;
            }

            return result;
        }

        public ICommand ExpandCollapseShowCommand { get; set; }
        public ICommand NavigateVenueOnMapCommand { get; set; }
        public ICommand ShowHideModalViewCommand { get; set; }
        public ICommand SetFavoriteCommand { get; set; }
        public ICommand UnsetFavoriteCommand { get; set; }

        public new VenueShowDataContext DataContext
        {
            get { return (VenueShowDataContext)base.DataContext; }
            set { base.DataContext = value; }
        }

        public bool IsExpanded
        {
            get { return _isExpanded; }
            set { SetIsExpanded(value, false); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this cell is before expanded one and the bottom border should be full width.
        /// </summary>
        public bool IsBeforeExpanded
        {
            get { return _isBeforeExpanded; }
            set { SetIsBeforeExpanded(value, false); }
        }

        public bool IsSeparatorVisible
        {
            get { return !Separator.Hidden; }
            set { Separator.Hidden = !value; }
        }

        public void SetIsExpanded(bool isExpanded, bool animated)
        {
            if (_isExpanded != isExpanded)
            {
                _isExpanded = isExpanded;

                UpdateConstraintConstants(animated);
                UpdateVisibility(animated);

                if (_isExpanded)
                {
                    _largeImageHelper.ImageUrl = null;
                    _largeImageAnimationDelay.Reset();
                    UpdateLargeImageState();
                }
                else
                {
                    _smallImageHelper.ImageUrl = null;
                    UpdateSmallImageState();
                }
            }
        }

        public void SetIsBeforeExpanded(bool isBeforeExpanded, bool animated)
        {
            if (_isBeforeExpanded != isBeforeExpanded)
            {
                _isBeforeExpanded = isBeforeExpanded;
                UpdateConstraintConstants(animated);
            }
        }

        public override void PrepareForReuse()
        {
            base.PrepareForReuse();

            IsExpanded = false;
            IsBeforeExpanded = false;
        }

        public override void WillMoveToSuperview(UIView newsuper)
        {
            base.WillMoveToSuperview(newsuper);

            if (newsuper == null)
            {
                DataContext = null;
                ExpandCollapseShowCommand = null;
                ShowHideModalViewCommand = null;

                DisposeGestures();
            }
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            if (_lastLayoutSize != Bounds.Size)
            {
                UpdateConstraintConstants(false);
                _lastLayoutSize = Bounds.Size;
            }
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            InitializeGestures();
            InitializeStyle();
        }

        protected override void OnDataContextChanged(object previousContext, object newContext)
        {
            _smallImageAnimationDelay.Reset();
            _smallImageHelper.ImageUrl = null;
            _largeImageHelper.ImageUrl = null;
            ThumbImageView.Image = null;

            if (DataContext != null &&
                DataContext.IsLogoVisible && !DataContext.Show.HasPictures())
            {
                ThumbImageView.Image = ThemeIcons.Circle;
            }

            ThumbLabel.Text = DataContext != null ? DataContext.Show.Title.GetAbbreviation(2) : null;
            TitleLabel.Text = DataContext != null  ? DataContext.Show.Title : null;
            LocationLabel.Text = DataContext != null ? DataContext.GetLocationString() : null;
            DetailsLabel.Text = DataContext != null && DataContext.Show.HasDetailsUrl()
                ? Localization.MoreInformation : null;

            UpdateTimeRelatedState();
            UpdateSmallImageState();
            UpdateLargeImageState();
            UpdateFavoriteState();
            UpdateVisibility(false);
            _lastLayoutSize = CGSize.Empty;
            SetNeedsLayout();
        }

        private void UpdateConstraintConstants(bool animated)
        {
            if (DataContext == null) return;

            this.UpdateConstraint(() =>
                {
                    TitleLeftConstraint.Constant = HorizontalGap + 
                        (!IsExpanded && DataContext.IsLogoVisible ? ImageSmallHeight + HorizontalGap : 0);

                    TitleAndLocationConstraint.Constant = IsExpanded &&
                        !string.IsNullOrWhiteSpace(DataContext.Show.Title) && DataContext.IsLocationAvailable
                            ? TextGap : 0;
                    
                    LocationAndDescriptionConstraint.Constant = IsExpanded &&
                        !string.IsNullOrWhiteSpace(DataContext.Show.Title) && 
                        !string.IsNullOrWhiteSpace(GetShowDescription(DataContext.Show, !DataContext.IsTimeVisible)) 
                            ? TextGap : 0;

                    var imageSpace = DataContext.Show.HasPictures() ? VerticalGap + ImageLargeHeight : 0;
                    var detailsSpace = DataContext.Show.HasDetailsUrl() ? VerticalGap : 0;
                    DescriptionAndDetailsSpaceConstraint.Constant = IsExpanded ? imageSpace + detailsSpace : 0;

                    if (IsExpanded && DataContext.Show.HasPictures())
                    {
                        ThumbTopConstraint.Constant = GetTextBlocksHeight(Frame.Width, DataContext.Show, 
                            DataContext.IsTimeVisible, DataContext.IsLocationAvailable) + VerticalGap;
                        ThumbLeftConstraint.Constant = HorizontalGap;
                        ThumbHeightConstraint.Constant = ImageLargeHeight;
                        ThumbWidthConstraint.Constant = GetImageProportionalWidth();
                    }
                    else if (!IsExpanded && DataContext.IsLogoVisible)
                    {
                        ThumbTopConstraint.Constant = 5;
                        ThumbLeftConstraint.Constant = 2;
                        ThumbHeightConstraint.Constant = ImageSmallHeight;
                        ThumbWidthConstraint.Constant = ImageSmallHeight;
                    }
                    else
                    {
                        ThumbHeightConstraint.Constant = ImageSmallHeight;
                        ThumbWidthConstraint.Constant = 0;
                    }

                    BottomBorderLeftConstraint.Constant = IsExpanded || IsBeforeExpanded 
                        ? 0 : HorizontalGap + ThumbWidthConstraint.Constant + 
                            (DataContext.IsLogoVisible ? HorizontalGap : 0) + HorizontalGap; 
                },
                animated);
        }

        private void UpdateSmallImageState()
        {
            var url = DataContext != null && DataContext.IsLogoVisible &&
                DataContext.Show.Pictures != null
                ? DataContext.Show.Pictures.Small : null;

            _smallImageHelper.ImageUrl = url;
        }

        private void UpdateLargeImageState()
        {
            var url = IsExpanded && DataContext != null &&
                DataContext.Show.Pictures != null
                ? DataContext.Show.Pictures.Medium : null;

            if (_largeImageHelper.ImageUrl != url)
            {
                if (url != null)
                {
                    ThumbImageView.StartProgress();
                }

                _largeImageHelper.ImageUrl = url;
            }
        }

        private void UpdateVisibility(bool animated)
        {
            if (DataContext == null) return;

            ThumbImageView.SetHidden(!DataContext.IsLogoVisible, animated);
            ThumbLabel.SetHidden(IsExpanded || !DataContext.IsLogoVisible || DataContext.Show.HasPictures(), animated);

            StartTimeLabel.SetHidden(!DataContext.IsTimeVisible, animated);
            EndTimeLabel.SetHidden(!DataContext.IsTimeVisible, animated);

            EndTimeLabel.TextColor = IsExpanded ? ThemeColors.ContentLightTextPassive : ThemeColors.BorderLight;

            var isLocationHidden = !IsExpanded || !DataContext.IsLocationAvailable;
            NavigateOnMapButton.SetHidden(isLocationHidden, animated);
            LocationLabel.SetHidden(isLocationHidden, animated);

            var isDescriptionHidden = !IsExpanded || GetShowDescription(DataContext.Show, !DataContext.IsTimeVisible) == null;
            DescriptionLabel.SetHidden(isDescriptionHidden, animated);

            var isDetailsHidden = !IsExpanded || !DataContext.Show.HasDetailsUrl();
            DetailsLabel.SetHidden(isDetailsHidden, animated);
            DetailsButton.SetHidden(isDetailsHidden, animated);
        }

        private void UpdateFavoriteState()
        {
            if (DataContext != null && DataContext.IsFavorite)
            {
                StarButton.SetImage(ThemeIcons.Star, UIControlState.Normal);
                //StarButton.TintColor = ThemeColors.Action;
            }
            else
            {
                StarButton.SetImage(ThemeIcons.StarOutline, UIControlState.Normal);
                //StarButton.TintColor = ThemeColors.BorderLight;
            }
        }

        private float GetImageProportionalWidth()
        {
            if (_largeImageHelper.ImageUrl != null &&
                ThumbImageView.HasImage() &&
                IsExpanded)
            {
                var imageSize = ThumbImageView.Image.Size;

                var width = Math.Min(
                    (float)(1.0 * imageSize.Width * ImageLargeHeight / imageSize.Height),
                    GetTitleBlockWidth(Frame.Width, DataContext.Show));
                return width;
            }

            return ImageLargeHeight;
        }

        private void InitializeGestures()
        {
            _cellTapGesture = new UITapGestureRecognizer(rec =>
                {
                    if (IsExpanded && DataContext.Show.HasPictures() &&
                        rec.LocatedInView(ThumbImageView))
                    {
                        var context = new ModalViewContext(
                            ModalViewKind.FullscreenImage, 
                            DataContext.Show.Pictures.Full);

                        if (ShowHideModalViewCommand != null &&
                            ShowHideModalViewCommand.CanExecute(context))
                        {
                            ShowHideModalViewCommand.Execute(context);
                        }
                    }
                    else if (IsExpanded && DataContext.Show.HasDetailsUrl() &&
                             rec.LocatedInView(this, 
                                 new CGRect(0, Bounds.Height - DetailsTapAreaHeight, 
                                     DetailsLabel.Frame.X + DetailsLabel.Frame.Width + HorizontalBorder, 
                                     DetailsTapAreaHeight)))
                    {
                        NavigateDetails();
                    }
                    else if (IsExpanded && DataContext.IsLocationAvailable &&
                        rec.LocatedInView(this, 
                            new CGRect(0, LocationLabel.Frame.Y, 
                                LocationLabel.Frame.X + LocationLabel.Frame.Width + HorizontalBorder, 
                                LocationLabel.Frame.Height)))
                    {
                        if (NavigateVenueOnMapCommand != null &&
                            NavigateVenueOnMapCommand.CanExecute(DataContext.TargetVenue))
                        {
                            NavigateVenueOnMapCommand.Execute(DataContext.TargetVenue);
                        }
                    }
                    else
                    {
                        if (ExpandCollapseShowCommand != null &&
                            ExpandCollapseShowCommand.CanExecute(DataContext.Show))
                        {
                            ExpandCollapseShowCommand.Execute(DataContext.Show);
                        }
                    }
                }) {
                NumberOfTouchesRequired = (uint)1,
                NumberOfTapsRequired = (uint)1
            };

            AddGestureRecognizer(_cellTapGesture);

            _starTapGesture = new UITapGestureRecognizer(() =>
                {
                    if (DataContext != null)
                    {
                        DataContext.IsFavorite = !DataContext.IsFavorite;
                        UpdateFavoriteState();

                        var tuple = new Tuple<int, Show>(DataContext.OrgEventId, DataContext.Show);

                        if (DataContext.IsFavorite)
                        {
                            if (SetFavoriteCommand.CanExecute(tuple))
                            {
                                SetFavoriteCommand.Execute(tuple);
                            }
                        }
                        else 
                        {
                            if (UnsetFavoriteCommand.CanExecute(tuple))
                            {
                                UnsetFavoriteCommand.Execute(tuple);
                            }
                        }
                    }
                });

            StarButton.AddGestureRecognizer(_starTapGesture);
        }

        private void DisposeGestures()
        {
            if (_cellTapGesture != null)
            {
                RemoveGestureRecognizer(_cellTapGesture);
                _cellTapGesture.Dispose();
                _cellTapGesture = null;
            }

            if (_starTapGesture != null)
            {
                StarButton.RemoveGestureRecognizer(_starTapGesture);
                _starTapGesture.Dispose();
                _starTapGesture = null;
            }
        }

        private void InitializeStyle()
        {   
            ThumbImageView.TintColor = ThemeColors.BorderLight;

            ThumbLabel.Font = Theme.VenueShowLogoFont;
            ThumbLabel.TextColor = ThemeColors.ContentDarkText;

            TitleLabel.Font = Theme.ContentFont;
            TitleLabel.TextColor = ThemeColors.ContentLightText;

            NavigateOnMapButton.SetImage(ThemeIcons.MapPinSmall, UIControlState.Normal);
            LocationLabel.Font = Theme.VenueShowDescriptionFont;
            LocationLabel.TextColor = ThemeColors.ContentLightTextPassive;

            DescriptionLabel.Font = Theme.VenueShowDescriptionFont;
            DescriptionLabel.TextColor = ThemeColors.ContentLightTextPassive;

            StartTimeLabel.Font = Theme.VenueShowTimeFont;
            StartTimeLabel.TextColor = ThemeColors.ContentLightTextPassive;

            EndTimeLabel.Font = Theme.VenueShowTimeFont;
            EndTimeLabel.TextColor = ThemeColors.BorderLight;

            DetailsButton.SetImage(ThemeIcons.Info, UIControlState.Normal);
            DetailsButton.TintColor = ThemeColors.Action;

            DetailsLabel.Font = Theme.VenueShowDetailsFont;
            DetailsLabel.TextColor = ThemeColors.Action;

            Separator.BackgroundColor = ThemeColors.ContentLightBackground;
            BackgroundView.BackgroundColor = ThemeColors.ContentLightBackground;
        }

        private void UpdateTimeRelatedState()
        {
            StartTimeLabel.Text = DataContext != null && DataContext.IsTimeVisible 
                ? GetShowTimeText(DataContext.Show.StartTime) : null;
            EndTimeLabel.Text = DataContext != null && DataContext.IsTimeVisible 
                ? GetShowTimeText(DataContext.Show.EndTime) : null;

            DescriptionLabel.Text = DataContext != null 
                ? GetShowDescription(DataContext.Show, !DataContext.IsTimeVisible) : null;

            if (DataContext == null)
            {
                TimeBackgroundView.BackgroundColor = UIColor.Clear;
                return;
            }

            switch (DataContext.Show.Status)
            {
                case ShowStatus.NotStarted:
                    TimeBackgroundView.BackgroundColor = UIColor.Clear;
                    TitleLabel.Font = Theme.ContentFont;
                    break;

                case ShowStatus.Started:
                    TimeBackgroundView.BackgroundColor = ThemeColors.Metadata;
                    TitleLabel.Font = Theme.ContentFont;
                    break;

                case ShowStatus.Finished:
                    TimeBackgroundView.BackgroundColor = UIColor.Clear;
                    TitleLabel.Font = DataContext.IsTimeVisible 
                        ? Theme.VenueShowFinishedFont : Theme.ContentFont;
                    break;
            }

            StartTimeLabel.Font = GetShowStartTimeFont(DataContext.Show.Status);
            EndTimeLabel.Font = GetShowEndTimeFont(DataContext.Show.Status);
        }

        private static string GetShowTimeText(DateTime? time)
        {
            return time.HasValue ? TimeFormatter.ToString(time.Value.ToNSDate()) : null;
        }

        private static UIFont GetShowStartTimeFont(ShowStatus status)
        {
            return status == ShowStatus.Finished 
                ? Theme.VenueShowFinishedTimeFont : Theme.VenueShowTimeFont;
        }

        private static UIFont GetShowEndTimeFont(ShowStatus status)
        {
            return status == ShowStatus.Finished 
                ? Theme.VenueShowFinishedEndTimeFont : Theme.VenueShowEndTimeFont;
        }

        private static string GetShowDescription(Show show, bool includeTime)
        {
            var result = default(string);

            if (includeTime)
            {
                var groupedShow = show as GroupedShow;
                if (groupedShow != null)
                {
                    result = groupedShow.Shows.Aggregate(string.Empty, 
                        (prev, s) => prev + (prev != string.Empty ? Environment.NewLine : string.Empty) +
                        GetShowDateTimeString(s));
                }
                else
                {
                    result = GetShowDateTimeString(show);
                }
            }

            var breakString = result != null && !string.IsNullOrWhiteSpace(show.Description) 
                ? Environment.NewLine + Environment.NewLine : null;
            
            return result + breakString + show.Description;
        }

        private static string GetShowDateTimeString(Show show)
        {
            var result = default(string);

            if (show.StartTime.HasValue || show.EndTime.HasValue)
            {
                result = (show.StartTime ?? show.EndTime).GetCurrentDayString() + "   " +
                         GetShowTimeText(show.StartTime) +
                         (show.StartTime.HasValue && show.EndTime.HasValue ? " - " : string.Empty) +
                         GetShowTimeText(show.EndTime);
            }

            return result;
        }

        partial void OnNavigateOnMapClick(NSObject sender)
        {
            if (NavigateVenueOnMapCommand != null &&
                NavigateVenueOnMapCommand.CanExecute(DataContext.TargetVenue))
            {
                NavigateVenueOnMapCommand.Execute(DataContext.TargetVenue);
            }
        }

        partial void OnDetailsButtonClick(NSObject sender)
        {
            NavigateDetails();
        }

        private void NavigateDetails()
        {
            var context = new ModalViewContext(
                ModalViewKind.Browser, 
                DataContext.Show.DetailsUrl);

            if (ShowHideModalViewCommand != null &&
                ShowHideModalViewCommand.CanExecute(context))
            {
                ShowHideModalViewCommand.Execute(context);
            }
        }
    }
}