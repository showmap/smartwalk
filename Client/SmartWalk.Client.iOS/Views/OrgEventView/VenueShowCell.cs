using System;
using System.Windows.Input;
using Cirrious.MvvmCross.Binding.Touch.Views;
using CoreGraphics;
using Foundation;
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

        private const float ImageSmallSize = 40f;
        private const float ImageHeight = 120f;
        private const float HorizontalGap = 5f;
        private const float VerticalGap = 12f;
        private const float TitleAndDescriptionGap = 3f;
        private const float TimeBorderGap = 8f;
        private const float BorderGap = 10f;
        private const float DetailsTapAreaHeight = 50f;

        private static readonly float ShowTitleTextHeight = 
            ScreenUtil.CalculateTextHeight(300, "Showp", Theme.ContentFont);
        private static readonly float DetailsTextHeight = 
            ScreenUtil.CalculateTextHeight(300, Localization.MoreInformation, Theme.VenueShowDetailsFont);

        public static readonly UINib Nib = UINib.FromName("VenueShowCell", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("VenueShowCell");

        public static readonly float DefaultHeight = VerticalGap + ShowTitleTextHeight + VerticalGap;

        private readonly MvxImageViewLoader _largeImageHelper;
        private readonly MvxResizedImageViewLoader _smallImageHelper;
        private readonly AnimationDelay _imageAnimationDelay = new AnimationDelay();

        private UITapGestureRecognizer _cellTapGesture;
        private bool _isExpanded;
        private bool _isLogoVisible;

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

        public static float CalculateCellHeight(
            nfloat frameWidth, 
            bool isExpanded,
            Show show)
        {
            if (isExpanded)
            {
                var cellHeight = 0f;
                var textHeight = GetTextBlocksHeight(frameWidth, show);

                var imageHeight = show.HasPicture() ? VerticalGap + ImageHeight : 0;
                var detailsHeight = show.HasDetailsUrl() ? VerticalGap + DetailsTextHeight : 0;

                // if no text, we still show times
                cellHeight += Math.Max(DefaultHeight, textHeight + imageHeight + detailsHeight + VerticalGap); 

                return cellHeight;
            }

            return DefaultHeight;
        }

        private static float GetTextBlocksHeight(nfloat frameWidth, Show show)
        {
            if (show == null) return 0;

            var titleHeight = show.Title != null
                ? ScreenUtil.CalculateTextHeight(
                    GetTitleBlockWidth(frameWidth, show), 
                    show.Title, 
                    Theme.ContentFont)
                : 0;

            var descriptionHeight = show.Description != null
                ? ScreenUtil.CalculateTextHeight(
                    GetDescriptionBlockWidth(frameWidth), 
                    show.Description, 
                    Theme.VenueShowDescriptionFont) + 
                (titleHeight > 0 ? TitleAndDescriptionGap : 0)
                : 0;

            var textHeight = VerticalGap + titleHeight + descriptionHeight;
            return textHeight;
        }

        private static float GetTitleBlockWidth(nfloat frameWidth, Show show)
        {
            // - Left Border Gap - Time Block Width (inc. Right Border Gap)
            return (float)(frameWidth - BorderGap - GetTimeBlockWidth(show));
        }

        private static float GetDescriptionBlockWidth(nfloat frameWidth)
        {
            // - Left Border Gap - Right Border Gap
            return (float)(frameWidth - BorderGap - BorderGap);
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

        public ICommand ShowImageFullscreenCommand { get; set; }
        public ICommand ExpandCollapseShowCommand { get; set; }
        public ICommand NavigateDetailsLinkCommand { get; set; }

        public new Show DataContext
        {
            get { return (Show)base.DataContext; }
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

            ThumbLabel.Text = DataContext.Title.GetAbbreviation(2);

            StartTimeLabel.Text = DataContext != null ? GetShowTimeText(DataContext.StartTime) : null;
            EndTimeLabel.Text = DataContext != null ? GetShowTimeText(DataContext.EndTime) : null;

            TitleLabel.Text = DataContext != null ? DataContext.Title : null;
            DescriptionLabel.Text = DataContext != null ? DataContext.Description : null;

            DetailsLabel.Text = DataContext != null && DataContext.HasDetailsUrl()
                ? Localization.MoreInformation : null;

            UpdateStatusStyle();
            UpdateSmallImageState();
            UpdateLargeImageState();
            UpdateConstraintConstants(false);
            UpdateVisibility(false);
        }

        private void UpdateConstraintConstants(bool animated)
        {
            TimeTopConstraint.Constant = DataContext != null &&
                (DataContext.StartTime.HasValue && DataContext.EndTime.HasValue)
                    ? 10 : 14;

            this.UpdateConstraint(() =>
                {
                    TitleLeftConstraint.Constant = HorizontalGap + (!IsExpanded && IsLogoVisible ? ImageSmallSize : 0);

                    TitleAndDescriptionSpaceConstraint.Constant = IsExpanded &&
                    DataContext.Title != null && DataContext.Description != null 
                            ? TitleAndDescriptionGap : 0;

                    var imageSpace = DataContext.HasPicture() ? VerticalGap + ImageHeight : 0;
                    var detailsSpace = DataContext.HasDetailsUrl() ? VerticalGap : 0;
                    DescriptionAndDetailsSpaceConstraint.Constant = IsExpanded ? imageSpace + detailsSpace : 0;

                    if (IsExpanded && DataContext.HasPicture())
                    {
                        ThumbTopConstraint.Constant = GetTextBlocksHeight(Frame.Width, DataContext) + VerticalGap;
                        ThumbLeftConstraint.Constant = HorizontalGap;
                        ThumbHeightConstraint.Constant = ImageHeight;
                        ThumbWidthConstraint.Constant = GetImageProportionalWidth();

                        ThumbImageView.MakeRect(new CGSize(
                            ThumbWidthConstraint.Constant, 
                            ThumbHeightConstraint.Constant));
                    }
                    else if (!IsExpanded && IsLogoVisible)
                    {
                        ThumbTopConstraint.Constant = 2;
                        ThumbLeftConstraint.Constant = 0;
                        ThumbHeightConstraint.Constant = ImageSmallSize;
                        ThumbWidthConstraint.Constant = ImageSmallSize;
                    }
                    else
                    {
                        ThumbHeightConstraint.Constant = ImageSmallSize;
                        ThumbWidthConstraint.Constant = 0;
                    }

                    BottomBorderLeftConstraint.Constant = IsExpanded 
                        ? 0 : HorizontalGap + ThumbWidthConstraint.Constant + HorizontalGap; 
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
                ? DataContext.Picture : null;

            _smallImageHelper.ImageUrl = url;
        }

        private void UpdateLargeImageState()
        {
            var url = IsExpanded && DataContext != null 
                ? DataContext.Picture : null;

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
            ThumbImageView.SetHidden(!IsLogoVisible || !DataContext.HasPicture(), animated);
            ThumbLabelView.SetHidden(IsExpanded || !IsLogoVisible || DataContext.HasPicture(), animated);

            var isDescriptionHidden = !IsExpanded || DataContext.Description == null;
            DescriptionLabel.SetHidden(isDescriptionHidden, animated);

            var isDetailsHidden = !IsExpanded || !DataContext.HasDetailsUrl();
            DetailsLabel.SetHidden(isDetailsHidden, animated);
            DetailsButton.SetHidden(isDetailsHidden, animated);
        }

        private float GetImageProportionalWidth()
        {
            if (_largeImageHelper.ImageUrl != null &&
                ThumbImageView.Image != null &&
                IsExpanded)
            {
                var imageSize = ThumbImageView.Image.Size;

                var width = Math.Min(
                    (float)(1.0 * imageSize.Width * ImageHeight / imageSize.Height),
                    GetTitleBlockWidth(Frame.Width, DataContext));
                return width;
            }

            return 150f;
        }

        private void InitializeGestures()
        {
            _cellTapGesture = new UITapGestureRecognizer(rec => 
            {
                if (IsExpanded && DataContext.HasPicture() && 
                    rec.LocatedInView(ThumbImageView))
                {
                    if (ShowImageFullscreenCommand != null &&
                        ShowImageFullscreenCommand.CanExecute(DataContext.Picture))
                    {
                        ShowImageFullscreenCommand.Execute(DataContext.Picture);
                    }
                }
                else if (IsExpanded && DataContext.HasDetailsUrl() && 
                        rec.LocatedInView(this, 
                            new CGRect(0, Bounds.Height - DetailsTapAreaHeight, 
                            DetailsLabel.Frame.X + DetailsLabel.Frame.Width + BorderGap, 
                            DetailsTapAreaHeight)))
                {
                    NavigateDetails();
                }
                else
                {
                    if (ExpandCollapseShowCommand != null &&
                        ExpandCollapseShowCommand.CanExecute(DataContext))
                    {
                        ExpandCollapseShowCommand.Execute(DataContext);
                    }
                }

            }) {
                NumberOfTouchesRequired = (uint)1,
                NumberOfTapsRequired = (uint)1
            };

            AddGestureRecognizer(_cellTapGesture);
        }

        private void DisposeGestures()
        {
            if (_cellTapGesture != null)
            {
                RemoveGestureRecognizer(_cellTapGesture);
                _cellTapGesture.Dispose();
                _cellTapGesture = null;
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

        private void UpdateStatusStyle()
        {
            if (DataContext == null)
            {
                TimeBackgroundView.BackgroundColor = UIColor.Clear;
                return;
            }

            switch (DataContext.Status)
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
                    TitleLabel.Font = Theme.VenueShowFinishedFont;
                    break;
            }

            StartTimeLabel.Font = GetShowStartTimeFont(DataContext.Status);
            EndTimeLabel.Font = GetShowEndTimeFont(DataContext.Status);
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

        partial void OnDetailsButtonClick(NSObject sender)
        {
            NavigateDetails();
        }

        private void NavigateDetails()
        {
            var contact = new Contact {
                Type = ContactType.Url,
                ContactText = DataContext.DetailsUrl
            };
            if (NavigateDetailsLinkCommand != null && 
                NavigateDetailsLinkCommand.CanExecute(contact))
            {
                NavigateDetailsLinkCommand.Execute(contact);
            }
        }
    }
}