using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Input;
using Cirrious.MvvmCross.Binding.Touch.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Client.Core.Model.DataContracts;
using SmartWalk.Client.Core.Resources;
using SmartWalk.Client.Core.Utils;
using SmartWalk.Shared.DataContracts;
using SmartWalk.Client.iOS.Resources;
using SmartWalk.Client.iOS.Utils;
using SmartWalk.Client.iOS.Views.Common.Base.Cells;
using SmartWalk.Client.iOS.Views.Common.GroupHeader;

namespace SmartWalk.Client.iOS.Views.OrgEventView
{
    public partial class VenueShowCell : TableCellBase
    {
        private static readonly string TimeFormat = CultureInfo.CurrentCulture.DateTimeFormat
            .ShortTimePattern.Replace(" tt", "t");
        private const string Space = " ";
        private const char M = 'm';

        private const float ImageHeight = 120f;
        private const float TimeBlockWidth = 60f;
        private const float VerticalGap = 12f;
        private const float TitleAndDescriptionGap = 3f;
        private const float TimeBorderGap = 8f;
        private const float BorderGap = 10f;

        private static readonly float ShowTitleTextHeight = 
            ScreenUtil.CalculateTextHeight(300, "Showp", Theme.VenueShowCellFont);
        private static readonly float DetailsTextHeight = 
            ScreenUtil.CalculateTextHeight(300, Localization.MoreInfo, Theme.VenueShowDetailsCellFont);

        public static readonly UINib Nib = UINib.FromName("VenueShowCell", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("VenueShowCell");

        public static readonly float DefaultHeight = VerticalGap + ShowTitleTextHeight + VerticalGap;

        private readonly AnimationDelay _animationDelay = new AnimationDelay();
        private readonly MvxImageViewLoader _imageHelper;
        private UITapGestureRecognizer _imageTapGesture;
        private UITapGestureRecognizer _cellTapGesture;
        private UITapGestureRecognizer _detailsTapGesture;
        private bool _isExpanded;
        private UIView _headerView;
        private UIView _subHeaderView;

        public VenueShowCell(IntPtr handle) : base(handle)
        {
            BackgroundView = new UIView { BackgroundColor = Theme.CellBackground };

            _imageHelper = new MvxImageViewLoader(
                () => ThumbImageView, 
                () => 
                {
                    if (_imageHelper.ImageUrl != null && 
                        ThumbImageView.Image != null)
                    {
                        UpdateConstraintConstants(false);

                        if (_animationDelay.Animate)
                        {
                            ThumbImageView.Hidden = true;
                            ThumbImageView.SetHidden(false, true);
                        }
                    }
                });
            _imageHelper.DefaultImagePath = Theme.DefaultImagePath;
            _imageHelper.ErrorImagePath = Theme.ErrorImagePath;
        }

        public static VenueShowCell Create()
        {
            return (VenueShowCell)Nib.Instantiate(null, null)[0];
        }

        public static float CalculateCellHeight(
            float frameWidth, 
            bool isExpanded, 
            bool isHeaderVisible,
            bool isSubHeaderVisible, 
            Show show)
        {
            if (isExpanded)
            {
                var cellHeight = (isHeaderVisible 
                    ? VenueHeaderView.DefaultHeight 
                    : 0f) + 
                    (isSubHeaderVisible 
                        ? GroupHeaderView.DefaultHeight 
                        : 0f);

                var titleHeight = show.Title != null
                    ? ScreenUtil.CalculateTextHeight(
                        GetTitleBlockWidth(frameWidth, show), 
                        show.Title, 
                        Theme.VenueShowCellFont)
                    : 0;

                var descriptionHeight = show.Description != null
                    ? ScreenUtil.CalculateTextHeight(
                        GetDescriptionBlockWidth(frameWidth), 
                        show.Description, 
                        Theme.VenueShowDescriptionCellFont) + 
                        (titleHeight > 0 ? TitleAndDescriptionGap : 0)
                    : 0;

                var logoHeight = show.HasPicture() ? VerticalGap + ImageHeight : 0;
                var detailsHeight = show.HasDetailsUrl() ? VerticalGap + DetailsTextHeight : 0;

                cellHeight += Math.Max(
                    DefaultHeight, 
                    VerticalGap + titleHeight + descriptionHeight + 
                    logoHeight + detailsHeight + VerticalGap); // if no text, we still show times

                return cellHeight;
            }

            return DefaultHeight;
        }

        private static float GetTitleBlockWidth(float frameWidth, Show show)
        {
            // - Left Border Gap - Time Block Width (inc. Right Border Gap)
            return frameWidth - BorderGap - GetTimeBlockWidth(show);
        }

        private static float GetDescriptionBlockWidth(float frameWidth)
        {
            // - Left Border Gap - Right Border Gap
            return frameWidth - BorderGap - BorderGap;
        }

        private static float GetTimeBlockWidth(Show show)
        {
            var result = TimeBorderGap; // Time Label right gap

            if (show != null && (show.StartTime.HasValue || show.EndTime.HasValue))
            {
                result += TimeBlockWidth;
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

        public Show NextShow { get; set; }

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

        public UIView HeaderView
        {
            get
            {
                return _headerView;
            }
            set
            {
                if (_headerView != value)
                {
                    if (_headerView != null)
                    {
                        _headerView.RemoveFromSuperview();
                    }

                    _headerView = value;
                    UpdateBackgroundColor();

                    if (_headerView != null)
                    {
                        _headerView.Frame = HeaderContainer.Bounds;

                        HeaderContainer.AddSubview(_headerView);
                    }
                }
            }
        }

        public UIView SubHeaderView
        {
            get
            {
                return _subHeaderView;
            }
            set
            {
                if (_subHeaderView != value)
                {
                    if (_subHeaderView != null)
                    {
                        _subHeaderView.RemoveFromSuperview();
                    }

                    _subHeaderView = value;
                    UpdateBackgroundColor();

                    if (_subHeaderView != null)
                    {
                        _subHeaderView.Frame = SubHeaderContainer.Bounds;

                        SubHeaderContainer.AddSubview(_subHeaderView);
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
                    UpateImageState();
                }
                else
                {
                    HeaderView = null;
                    SubHeaderView = null;
                }
            }
        }

        public override void PrepareForReuse()
        {
            base.PrepareForReuse();

            IsExpanded = false;
        }

        public override void WillMoveToSuperview(UIView newsuper)
        {
            base.WillMoveToSuperview(newsuper);

            if (newsuper == null)
            {
                ExpandCollapseShowCommand = null;
                ShowImageFullscreenCommand = null;
                NavigateDetailsLinkCommand = null;
                HeaderView = null;
                SubHeaderView = null;

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
            _animationDelay.Reset();

            ThumbImageView.Image = null;
            _imageHelper.ImageUrl = null;

            StartTimeLabel.Text = DataContext != null && DataContext.StartTime.HasValue
                ? DataContext.StartTime.Value.ToString(TimeFormat)
                : null;
            EndTimeLabel.Text = DataContext != null && DataContext.EndTime.HasValue
                ? DataContext.EndTime.Value.ToString(TimeFormat)
                : null;

            TitleLabel.Text = DataContext != null ? DataContext.Title : null;
            DescriptionLabel.Text = DataContext != null ? DataContext.Description : null;

            UpdateStatusStyle();
            UpateImageState();
            UpdateConstraintConstants(false);
            UpdateVisibility(false);
        }

        private void UpdateConstraintConstants(bool animated)
        {
            HeaderHeightConstraint.Constant = 
                IsExpanded &&
                HeaderView != null
                ? VenueHeaderView.DefaultHeight
                : 0;

            SubHeaderHeightConstraint.Constant = 
                IsExpanded &&
                SubHeaderView != null
                ? GroupHeaderView.DefaultHeight
                : 0;

            TitleRightConstraint.Constant = GetTimeBlockWidth(DataContext);

            if (IsExpanded &&
                DataContext.Title != null &&
                DataContext.Description != null)
            {
                TitleAndDescriptionSpaceConstraint.Constant = TitleAndDescriptionGap;
            }
            else
            {
                TitleAndDescriptionSpaceConstraint.Constant = 0;
            }

            TimeTopConstraint.Constant = DataContext != null &&
                (DataContext.StartTime.HasValue && DataContext.EndTime.HasValue)
                    ? 8 : 13;

            this.UpdateConstraint(() =>
                {
                    if (IsExpanded && DataContext.HasPicture())
                    {
                        ImageHeightConstraint.Constant = ImageHeight;
                        ImageWidthConstraint.Constant = GetImageProportionalWidth();
                        DescriptionAndImageSpaceConstraint.Constant = VerticalGap;
                    }
                    else
                    {
                        ImageHeightConstraint.Constant = 0;
                        DescriptionAndImageSpaceConstraint.Constant = 0;
                    }
                },
                IsExpanded && animated);

            this.UpdateConstraint(() =>
                {
                    if (IsExpanded && DataContext.HasDetailsUrl())
                    {
                        DetailsHeightConstraint.Constant = DetailsTextHeight;
                        ImageAndDetailsSpaceConstraint.Constant = VerticalGap;
                    }
                    else
                    {
                        DetailsHeightConstraint.Constant = 0;
                        ImageAndDetailsSpaceConstraint.Constant = 0;
                    }
                },
                animated);
        }

        private void UpateImageState()
        {
            var url = IsExpanded && DataContext != null 
                ? DataContext.Picture : null;

            if (_imageHelper.ImageUrl != url)
            {
                if (url != null)
                {
                    ThumbImageView.StartProgress();
                }

                _imageHelper.ImageUrl = url;
            }
        }

        private void UpdateVisibility(bool animated)
        {
            var isHeaderHidden = !IsExpanded || HeaderView == null;
            HeaderContainer.SetHidden(isHeaderHidden, animated);

            var isSubHeaderHidden = !IsExpanded || SubHeaderView == null;
            SubHeaderContainer.SetHidden(isSubHeaderHidden, animated);

            var isDescriptionHidden = !IsExpanded || DataContext.Description == null;
            DescriptionLabel.SetHidden(isDescriptionHidden, animated);

            var isImageHidden = !IsExpanded || !DataContext.HasPicture();
            ThumbImageView.SetHidden(isImageHidden, !isImageHidden && animated, 0.6);

            var isDetailsHidden = !IsExpanded || !DataContext.HasDetailsUrl();
            DetailsLabel.SetHidden(isDetailsHidden, animated);
        }

        private float GetImageProportionalWidth()
        {
            if (_imageHelper.ImageUrl != null &&
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
            _cellTapGesture = new UITapGestureRecognizer(() => {
                if (ExpandCollapseShowCommand != null &&
                    ExpandCollapseShowCommand.CanExecute(DataContext))
                {
                    ExpandCollapseShowCommand.Execute(DataContext);
                }
            }) {
                NumberOfTouchesRequired = (uint)1,
                NumberOfTapsRequired = (uint)1
            };

            _imageTapGesture = new UITapGestureRecognizer(() => {
                if (ShowImageFullscreenCommand != null &&
                    ShowImageFullscreenCommand.CanExecute(DataContext.Picture))
                {
                    ShowImageFullscreenCommand.Execute(DataContext.Picture);
                }
            }) {
                NumberOfTouchesRequired = (uint)1,
                NumberOfTapsRequired = (uint)1
            };

            var uITapGestureRecognizer = new UITapGestureRecognizer(() => 
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
            });
            uITapGestureRecognizer.NumberOfTouchesRequired = (uint)1;
            uITapGestureRecognizer.NumberOfTapsRequired = (uint)1;
            _detailsTapGesture = uITapGestureRecognizer;

            _cellTapGesture.RequireGestureRecognizerToFail(_imageTapGesture);
            _cellTapGesture.RequireGestureRecognizerToFail(_detailsTapGesture);

            AddGestureRecognizer(_cellTapGesture);
            ThumbImageView.AddGestureRecognizer(_imageTapGesture);
            DetailsLabel.AddGestureRecognizer(_detailsTapGesture);
        }

        private void DisposeGestures()
        {
            if (_cellTapGesture != null)
            {
                RemoveGestureRecognizer(_cellTapGesture);
                _cellTapGesture.Dispose();
                _cellTapGesture = null;
            }

            if (_imageTapGesture != null)
            {
                ThumbImageView.RemoveGestureRecognizer(_imageTapGesture);
                _imageTapGesture.Dispose();
                _imageTapGesture = null;
            }

            if (_detailsTapGesture != null)
            {
                DetailsLabel.RemoveGestureRecognizer(_detailsTapGesture);
                _detailsTapGesture.Dispose();
                _detailsTapGesture = null;
            }
        }

        private void InitializeStyle()
        {
            TitleLabel.Font = Theme.VenueShowCellFont;
            TitleLabel.TextColor = Theme.CellText;

            DescriptionLabel.Font = Theme.VenueShowDescriptionCellFont;
            DescriptionLabel.TextColor = Theme.CellTextPassive;

            StartTimeLabel.Font = Theme.VenueShowCellTimeFont;
            StartTimeLabel.TextColor = Theme.CellText;

            EndTimeLabel.Font = Theme.VenueShowCellTimeFont;
            EndTimeLabel.TextColor = Theme.CellTextPassive;

            DetailsLabel.Font = Theme.VenueShowDetailsCellFont;
            DetailsLabel.TextColor = Theme.HyperlinkText;
            DetailsLabel.Text = Localization.MoreInfo;

            HeaderContainer.Layer.ShadowColor = UIColor.Black.CGColor;
            HeaderContainer.Layer.ShadowOffset = new SizeF(0, 2);
            HeaderContainer.Layer.ShadowOpacity = 0.1f;
        }

        private void UpdateStatusStyle()
        {
            if (DataContext == null)
            {
                TimeBackgroundView.BackgroundColor = UIColor.Clear;
                return;
            }

            var status = DataContext.GetStatus(NextShow);

            switch (status)
            {
                case ShowStatus.NotStarted:
                    TimeBackgroundView.BackgroundColor = UIColor.Clear;
                    break;

                case ShowStatus.Started:
                    TimeBackgroundView.BackgroundColor = Theme.HyperlinkText;
                    break;

                case ShowStatus.Finished:
                    TimeBackgroundView.BackgroundColor = UIColor.Clear;
                    break;
            }

            StartTimeLabel.Font = status == ShowStatus.Finished 
                ? Theme.VenueShowCellFinishedTimeFont : Theme.VenueShowCellTimeFont;
            EndTimeLabel.Font = status == ShowStatus.Finished 
                ? Theme.VenueShowCellFinishedEndTimeFont : Theme.VenueShowCellEndTimeFont;
        }

        private void UpdateBackgroundColor()
        {
            BackgroundView.BackgroundColor = 
                HeaderView != null || SubHeaderView != null
                ? Theme.CellSemiHighlight 
                : Theme.CellBackground;
        }
    }
}