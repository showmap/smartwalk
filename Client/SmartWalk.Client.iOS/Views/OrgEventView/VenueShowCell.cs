using System;
using System.Windows.Input;
using Cirrious.MvvmCross.Binding.Touch.Views;
using CoreGraphics;
using Foundation;
using SmartWalk.Client.Core.Model;
using SmartWalk.Client.Core.Model.DataContracts;
using SmartWalk.Client.Core.Resources;
using SmartWalk.Client.Core.Utils;
using SmartWalk.Client.iOS.Resources;
using SmartWalk.Client.iOS.Utils;
using SmartWalk.Client.iOS.Utils.MvvmCross;
using SmartWalk.Client.iOS.Views.Common.Base.Cells;
using SmartWalk.Shared.DataContracts;
using SmartWalk.Shared.Utils;
using UIKit;

namespace SmartWalk.Client.iOS.Views.OrgEventView
{
    public partial class VenueShowCell : TableCellBase
    {
        private static readonly NSDateFormatter TimeFormatter = new NSDateFormatter {
            DateStyle = NSDateFormatterStyle.None,
            TimeStyle = NSDateFormatterStyle.Short,
            TimeZone = NSTimeZone.DefaultTimeZone
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

        private readonly MvxImageViewLoader _largeImageHelper;
        private readonly MvxResizedImageViewLoader _smallImageHelper;
        private readonly AnimationDelay _imageAnimationDelay = new AnimationDelay();

        private UITapGestureRecognizer _cellTapGesture;
        private UITapGestureRecognizer _mapTapGesture;
        private UITapGestureRecognizer _starTapGesture;
        private bool _isExpanded;
        private bool _isLogoVisible;
        private bool _isTimeVisible = true;

        public VenueShowCell(IntPtr handle) : base(handle)
        {
            BackgroundView = new UIView { BackgroundColor = ThemeColors.ContentLightBackground };

            _smallImageHelper = new MvxResizedImageViewLoader(
                () => ThumbImageView,
                () => 
                {
                    if (ThumbImageView != null && ThumbImageView.ProgressEnded())
                    {
                        var noImage = !ThumbImageView.HasImage();
                        ThumbImageView.SetHidden(noImage, _imageAnimationDelay.Animate);

                        // showing abbr if image couldn't be loaded
                        ThumbLabelView.SetHidden(!noImage, false);
                    }
                });

            _largeImageHelper = new MvxImageViewLoader(
                () => ThumbImageView, 
                () => 
                {
                    if (_largeImageHelper.ImageUrl != null && 
                        ThumbImageView.Image != null)
                    {
                        UpdateConstraintConstants(false);
                    }
                });
            _largeImageHelper.DefaultImagePath = Theme.DefaultImagePath;
            _largeImageHelper.ErrorImagePath = Theme.ErrorImagePath;
        }

        public static VenueShowCell Create()
        {
            return (VenueShowCell)Nib.Instantiate(null, null)[0];
        }

        public static float CalculateCellHeight(nfloat frameWidth, bool isExpanded,
            Show show, bool showTime = true, bool includeLocation = false)
        {
            if (isExpanded)
            {
                var cellHeight = 0f;
                var textHeight = GetTextBlocksHeight(frameWidth, show, showTime, includeLocation);

                var imageHeight = show.HasPicture() ? VerticalGap + ImageLargeHeight : 0f;
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
                show.Title != null
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
                showDescription != null
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
        public ICommand ShowImageFullscreenCommand { get; set; }
        public ICommand NavigateDetailsLinkCommand { get; set; }

        public new VenueShowDataContext DataContext
        {
            get { return (VenueShowDataContext)base.DataContext; }
            set { base.DataContext = value; }
        }

        public bool IsExpanded
        {
            get
            {
                return _isExpanded;
            }
            set
            {
                SetIsExpanded(value, false);
            }
        }

        public bool IsLogoVisible
        {
            get
            {
                return _isLogoVisible;
            }
            set
            {
                if (_isLogoVisible != value)
                {
                    _isLogoVisible = value;

                    UpdateConstraintConstants(false);
                    UpdateVisibility(false);

                    if (_isLogoVisible)
                    {
                        UpdateSmallImageState();
                    }
                }
            }
        }

        public bool IsTimeVisible
        {
            get
            {
                return _isTimeVisible;
            }
            set
            {
                if (_isTimeVisible != value)
                {
                    _isTimeVisible = value;

                    UpdateTimeRelatedState();
                    UpdateConstraintConstants(false);
                    UpdateVisibility(false);
                }
            }
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
                    UpdateLargeImageState();
                }
            }
        }

        public override void PrepareForReuse()
        {
            base.PrepareForReuse();

            IsExpanded = false;
            IsLogoVisible = false;
            IsTimeVisible = true;
        }

        public override void WillMoveToSuperview(UIView newsuper)
        {
            base.WillMoveToSuperview(newsuper);

            if (newsuper == null)
            {
                ExpandCollapseShowCommand = null;
                ShowImageFullscreenCommand = null;
                NavigateDetailsLinkCommand = null;

                DisposeGestures();
            }
        }

        protected override void OnInitialize()
        {
            InitializeGestures();
            InitializeStyle();
        }

        protected override void OnDataContextChanged(object previousContext, object newContext)
        {
            _imageAnimationDelay.Reset();
            _smallImageHelper.ImageUrl = null;
            _largeImageHelper.ImageUrl = null;
            ThumbImageView.Image = null;

            ThumbLabel.Text = DataContext.Show.Title.GetAbbreviation(2);

            TitleLabel.Text = DataContext != null  ? DataContext.Show.Title : null;
            LocationLabel.Text = DataContext != null && DataContext.Venue != null 
                ? DataContext.Venue.DisplayName() : null;

            DetailsLabel.Text = DataContext != null && DataContext.Show.HasDetailsUrl()
                ? Localization.MoreInformation : null;

            UpdateTimeRelatedState();
            UpdateSmallImageState();
            UpdateLargeImageState();
            UpdateConstraintConstants(false);
            UpdateVisibility(false);
        }

        private void UpdateConstraintConstants(bool animated)
        {
            TimeTopConstraint.Constant = DataContext != null &&
                (DataContext.Show.StartTime.HasValue && DataContext.Show.EndTime.HasValue)
                    ? 13 : 17;

            this.UpdateConstraint(() =>
                {
                    TitleLeftConstraint.Constant = HorizontalGap + 
                        (!IsExpanded && IsLogoVisible ? ImageSmallHeight + HorizontalGap : 0);

                    TitleAndLocationConstraint.Constant = IsExpanded &&
                        DataContext.Show.Title != null && DataContext.Venue != null
                            ? TextGap : 0;
                    
                    LocationAndDescriptionConstraint.Constant = IsExpanded &&
                        DataContext.Show.Title != null && GetShowDescription(DataContext.Show, !IsTimeVisible) != null 
                            ? TextGap : 0;

                    var imageSpace = DataContext.Show.HasPicture() ? VerticalGap + ImageLargeHeight : 0;
                    var detailsSpace = DataContext.Show.HasDetailsUrl() ? VerticalGap : 0;
                    DescriptionAndDetailsSpaceConstraint.Constant = IsExpanded ? imageSpace + detailsSpace : 0;

                    if (IsExpanded && DataContext.Show.HasPicture())
                    {
                        ThumbTopConstraint.Constant = GetTextBlocksHeight(Frame.Width, DataContext.Show, 
                            IsTimeVisible, DataContext.Venue != null) + VerticalGap;
                        ThumbLeftConstraint.Constant = HorizontalGap;
                        ThumbHeightConstraint.Constant = ImageLargeHeight;
                        ThumbWidthConstraint.Constant = GetImageProportionalWidth();

                        ThumbImageView.MakeRect(new CGSize(
                            ThumbWidthConstraint.Constant, 
                            ThumbHeightConstraint.Constant));
                    }
                    else if (!IsExpanded && IsLogoVisible)
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

                    BottomBorderLeftConstraint.Constant = IsExpanded 
                        ? 0 : HorizontalGap + ThumbWidthConstraint.Constant + 
                            (IsLogoVisible ? HorizontalGap : 0) + HorizontalGap; 
                },
                animated,
                () =>
                    {
                        if (!IsExpanded)
                        {
                            ThumbImageView.MakeRound();
                        }
                    });
        }

        private void UpdateSmallImageState()
        {
            var url = IsLogoVisible && DataContext != null 
                ? DataContext.Show.Picture : null;

            _smallImageHelper.ImageUrl = url;
        }

        private void UpdateLargeImageState()
        {
            var url = IsExpanded && DataContext != null 
                ? DataContext.Show.Picture : null;

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

            ThumbImageView.SetHidden(!IsLogoVisible || !DataContext.Show.HasPicture(), animated);
            ThumbLabelView.SetHidden(IsExpanded || !IsLogoVisible || DataContext.Show.HasPicture(), animated);

            StartTimeLabel.SetHidden(!IsTimeVisible, animated);
            EndTimeLabel.SetHidden(!IsTimeVisible, animated);

            var isLocationHidden = !IsExpanded || DataContext.Venue == null;
            NavigateOnMapButton.SetHidden(isLocationHidden, animated);
            LocationLabel.SetHidden(isLocationHidden, animated);

            var isDescriptionHidden = !IsExpanded || GetShowDescription(DataContext.Show, !IsTimeVisible) == null;
            DescriptionLabel.SetHidden(isDescriptionHidden, animated);

            var isDetailsHidden = !IsExpanded || !DataContext.Show.HasDetailsUrl();
            DetailsLabel.SetHidden(isDetailsHidden, animated);
            DetailsButton.SetHidden(isDetailsHidden, animated);
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

            return 150f;
        }

        private void InitializeGestures()
        {
            _cellTapGesture = new UITapGestureRecognizer(rec =>
                {
                    if (IsExpanded && DataContext.Show.HasPicture() &&
                        rec.LocatedInView(ThumbImageView))
                    {
                        if (ShowImageFullscreenCommand != null &&
                            ShowImageFullscreenCommand.CanExecute(DataContext.Show.Picture))
                        {
                            ShowImageFullscreenCommand.Execute(DataContext.Show.Picture);
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

            _mapTapGesture = new UITapGestureRecognizer(() =>
                {
                    if (NavigateVenueOnMapCommand != null &&
                        NavigateVenueOnMapCommand.CanExecute(DataContext.Venue))
                    {
                        NavigateVenueOnMapCommand.Execute(DataContext.Venue);
                    }
                });

            NavigateOnMapButton.AddGestureRecognizer(_mapTapGesture);

            _starTapGesture = new UITapGestureRecognizer(() =>
                {
                    StarButton.SetImage(ThemeIcons.Star, UIControlState.Normal);
                    StarButton.TintColor = ThemeColors.Action;
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

            if (_mapTapGesture != null)
            {
                NavigateOnMapButton.RemoveGestureRecognizer(_mapTapGesture);
                _mapTapGesture.Dispose();
                _mapTapGesture = null;
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
            ThumbImageView.MakeRound();

            ThumbLabel.Font = Theme.VenueShowLogoFont;
            ThumbLabel.TextColor = ThemeColors.ContentDarkText;

            ThumbLabelView.MakeRound();
            ThumbLabelView.BackgroundColor = ThemeColors.BorderLight;

            TitleLabel.Font = Theme.ContentFont;
            TitleLabel.TextColor = ThemeColors.ContentLightText;

            StarButton.SetImage(ThemeIcons.StarOutline, UIControlState.Normal);
            StarButton.TintColor = ThemeColors.BorderLight;

            NavigateOnMapButton.SetImage(ThemeIcons.MapPinSmall, UIControlState.Normal);
            LocationLabel.Font = Theme.VenueShowDescriptionFont;
            LocationLabel.TextColor = ThemeColors.ContentLightTextPassive;

            DescriptionLabel.Font = Theme.VenueShowDescriptionFont;
            DescriptionLabel.TextColor = ThemeColors.ContentLightTextPassive;

            StartTimeLabel.Font = Theme.VenueShowTimeFont;
            StartTimeLabel.TextColor = ThemeColors.ContentLightTextPassive;

            EndTimeLabel.Font = Theme.VenueShowTimeFont;
            EndTimeLabel.TextColor = ThemeColors.ContentLightTextPassive;

            DetailsButton.SetImage(ThemeIcons.Info, UIControlState.Normal);
            DetailsButton.TintColor = ThemeColors.Action;

            DetailsLabel.Font = Theme.VenueShowDetailsFont;
            DetailsLabel.TextColor = ThemeColors.Action;

            BackgroundView.BackgroundColor = ThemeColors.ContentLightBackground;
        }

        private void UpdateTimeRelatedState()
        {
            StartTimeLabel.Text = DataContext != null && IsTimeVisible 
                ? GetShowTimeText(DataContext.Show.StartTime) : null;
            EndTimeLabel.Text = DataContext != null && IsTimeVisible 
                ? GetShowTimeText(DataContext.Show.EndTime) : null;

            DescriptionLabel.Text = DataContext != null 
                ? GetShowDescription(DataContext.Show, !IsTimeVisible) : null;

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
                    TitleLabel.Font = IsTimeVisible ? Theme.VenueShowFinishedFont : Theme.ContentFont;
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
            var result = includeTime 
                ? (show.StartTime ?? show.EndTime).GetCurrentDayString() + "   " +
                         GetShowTimeText(show.StartTime) +
                         (show.StartTime.HasValue && show.EndTime.HasValue ? " - " : string.Empty) +
                         GetShowTimeText(show.EndTime) +
                         ((show.StartTime.HasValue || show.EndTime.HasValue) && show.Description != null 
                    ? Environment.NewLine + Environment.NewLine : string.Empty) +
                         show.Description
                : show.Description;
            return result;
        }

        partial void OnDetailsButtonClick(NSObject sender)
        {
            NavigateDetails();
        }

        private void NavigateDetails()
        {
            var contact = new Contact {
                Type = ContactType.Url,
                ContactText = DataContext.Show.DetailsUrl
            };
            if (NavigateDetailsLinkCommand != null && 
                NavigateDetailsLinkCommand.CanExecute(contact))
            {
                NavigateDetailsLinkCommand.Execute(contact);
            }
        }
    }

    public class VenueShowDataContext
    {
        public VenueShowDataContext(Show show, Venue venue = null)
        {
            Venue = venue;
            Show = show;
        }
        
        public Venue Venue { get; private set; }
        public Show Show { get; private set; }
    }
}