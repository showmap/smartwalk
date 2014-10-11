using System;
using System.Drawing;
using System.Windows.Input;
using Cirrious.MvvmCross.Binding.Touch.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Client.Core.Model.DataContracts;
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
        public static readonly UINib Nib = UINib.FromName("VenueShowCell", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("VenueShowCell");

        public readonly static float DefaultHeight = VerticalGap + 
            (float)Math.Ceiling(Theme.VenueShowCellFont.LineHeight) + VerticalGap;

        private static readonly string TimeFormat = "{0:t}";
        private static readonly string Space = " ";
        private static readonly char M = 'm';

        private const float ImageHeight = 100f;
        private const float TimeBlockWidth = 106f;
        private const float VerticalGap = 12f;
        private const float BorderGap = 8f;

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
                        SetNeedsLayout();
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

                var showText = GetShowText(show);  
                if (showText.Length > 0)
                {
                    var logoHeight = show.HasPicture() ? VerticalGap + ImageHeight : 0;
                    var detailsHeight = show.HasDetailsUrl()
                        ? VerticalGap + (float)Math.Ceiling(Theme.VenueShowCellFont.LineHeight) 
                        : 0;
                    var textHeight =    
                        (float)Math.Ceiling(CalculateTextHeight(GetTextWidth(frameWidth, show), showText));
                    cellHeight += Math.Max(
                        DefaultHeight, 
                        VerticalGap + textHeight + logoHeight + detailsHeight + VerticalGap); // if no text, we still show times
                }

                return cellHeight;
            }

            return DefaultHeight;
        }

        private static float CalculateTextHeight(float frameWidth, NSAttributedString text)
        {
            if (text.Length > 0)
            {
                var frameSize = new SizeF(frameWidth, float.MaxValue);
                var textSize = 
                    text.GetBoundingRect(
                       frameSize, 
                       NSStringDrawingOptions.UsesLineFragmentOrigin |
                       NSStringDrawingOptions.UsesFontLeading,
                       null);

                return textSize.Height;
            }

            return 0;
        }

        private static float GetTextWidth(float frameWidth, Show show)
        {
            // - Left Border Gap - Time Block Width (inc. Right Border Gap)
            return frameWidth - BorderGap - GetTimeBlockWidth(show);
        }

        private static float GetTimeBlockWidth(Show show)
        {
            var result = BorderGap; // Time Label right gap

            if (show != null)
            {
                if (show.StartTime.HasValue && show.EndTime.HasValue)
                {
                    result += TimeBlockWidth;
                }
                else if (show.StartTime.HasValue || show.EndTime.HasValue)
                {
                    result += (float)Math.Ceiling((float)TimeBlockWidth / 2);
                }
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
                if (_isExpanded != value)
                {
                    _isExpanded = value;
                    UpateImageState();
                    UpdateVisibility();
                    UpdateConstraints();

                    if (!_isExpanded)
                    {
                        HeaderView = null;
                    }
                }
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
                    UpdateVisibility();
                    UpdateConstraints();
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
                    UpdateVisibility();
                    UpdateConstraints();
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

        public override void PrepareForReuse()
        {
            base.PrepareForReuse();

            IsExpanded = false;
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            UpdateConstraints();
        }

        public override void UpdateConstraints()
        {
            base.UpdateConstraints();

            var calculatedHeight = 
                DataContext != null 
                    ? CalculateCellHeight(
                        Frame.Width, 
                        IsExpanded, 
                        HeaderView != null,
                        SubHeaderView != null,
                        DataContext)
                    : 0;

            HeaderHeightConstraint.Constant = 
                IsExpanded && 
                HeaderView != null &&
                Frame.Height >= calculatedHeight
                    ? VenueHeaderView.DefaultHeight
                    : 0;

            SubHeaderHeightConstraint.Constant = 
                IsExpanded && 
                SubHeaderView != null &&
                Frame.Height >= calculatedHeight
                    ? GroupHeaderView.DefaultHeight
                    : 0;

            DescriptionRightConstraint.Constant = GetTimeBlockWidth(DataContext);

            if (IsExpanded &&
                DataContext.HasPicture() &&
                Frame.Height >= calculatedHeight)
            {
                ImageHeightConstraint.Constant = ImageHeight;
                ImageWidthConstraint.Constant = GetImageProportionalWidth();
                DescriptionAndImageSpaceConstraint.Constant = VerticalGap;
            }
            else
            {
                ImageHeightConstraint.Constant = 0;
                ImageWidthConstraint.Constant = 0;
                DescriptionAndImageSpaceConstraint.Constant = 0;
            }

            if (IsExpanded && 
                DataContext.HasDetailsUrl() &&
                Frame.Height >= calculatedHeight)
            {
                DetailsHeightConstraint.Constant = 
                    (float)Math.Ceiling(Theme.VenueShowCellFont.LineHeight);
                ImageAndDetailsSpaceConstraint.Constant = VerticalGap;
            }
            else
            {
                DetailsHeightConstraint.Constant = 0;
                ImageAndDetailsSpaceConstraint.Constant = 0;
            }

            /*EndTimeLeftSpaceConstraint.Constant =
                EndTimeLabel.Text != null &&
                    EndTimeLabel.Text.StartsWith("1", StringComparison.InvariantCultureIgnoreCase)
                    ? 2 : 3;
            EndTimeRightSpaceConstraint.Constant =
                EndTimeLabel.Text != null &&
                    EndTimeLabel.Text.StartsWith("1", StringComparison.InvariantCultureIgnoreCase)
                    ? 9 : 8;*/
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
            ThumbImageView.Image = null;
            _imageHelper.ImageUrl = null;

            var leftRightTimes = GetLeftAndRightTimeTexts(DataContext);

            StartTimeLabel.AttributedText = leftRightTimes.Item1;
            EndTimeLabel.AttributedText = leftRightTimes.Item2;

            DescriptionLabel.AttributedText = DataContext != null 
                ? GetShowText(DataContext) 
                : new NSAttributedString();

            UpdateClockIcon();
            UpateImageState();
            UpdateVisibility();
            UpdateConstraints();
        }

        private static NSAttributedString GetTimeText(DateTime time, ShowStatus status)
        {
            var timeStr = String.Format(TimeFormat, time)
                .Replace(Space, string.Empty).ToLower();

            var index = timeStr.IndexOf(M);
            var result = new NSMutableAttributedString(
                timeStr,
                Theme.VenueShowCellTimeFont);

            if (index > 0)
            {
                var ampmIndex = index - 1;

                result.SetAttributes(
                    new UIStringAttributes 
                    { 
                        Font = status == ShowStatus.Finished 
                            ? Theme.VenueShowCellFinishedTimeFont 
                            : Theme.VenueShowCellTimeFont
                    }, 
                    new NSRange(0, ampmIndex));
                result.SetAttributes(
                    new UIStringAttributes { Font = Theme.VenueShowCellTimeAmPmFont },
                    new NSRange(ampmIndex, timeStr.Length - ampmIndex));
            }

            return result;
        }

        private static NSAttributedString GetShowText(Show show)
        {
            var result = new NSMutableAttributedString(
                show.GetText(),
                Theme.VenueShowCellFont,
                Theme.CellText
            );

            if (show.Title != null && show.Description != null)
            {
                result.SetAttributes(
                    new UIStringAttributes 
                    { 
                        Font = Theme.VenueShowDescriptionCellFont,
                        ForegroundColor = Theme.CellTextPassive
                    },
                    new NSRange(
                        show.Title.Length, 
                        result.Length - show.Title.Length));
            }

            return result;
        }

        private static Tuple<NSAttributedString, NSAttributedString> GetLeftAndRightTimeTexts(Show show)
        {
            if (show != null)
            {
                if (show.StartTime.HasValue && show.EndTime.HasValue)
                {
                    return new Tuple<NSAttributedString, NSAttributedString>(
                        GetTimeText(show.StartTime.Value, show.GetStatus()),
                        GetTimeText(show.EndTime.Value, show.GetStatus()));
                }

                // putting start time to the right, for better layout
                if (show.StartTime.HasValue)
                {
                    return new Tuple<NSAttributedString, NSAttributedString>(
                        new NSAttributedString(),
                        GetTimeText(show.StartTime.Value, show.GetStatus()));
                }

                if (show.EndTime.HasValue)
                {
                    return new Tuple<NSAttributedString, NSAttributedString>(
                        new NSAttributedString(),
                        GetTimeText(show.EndTime.Value, show.GetStatus()));
                }
            }

            return new Tuple<NSAttributedString, NSAttributedString>(
                new NSAttributedString(), new NSAttributedString());
        }

        private void UpateImageState()
        {
            var url = IsExpanded && DataContext != null 
                ? DataContext.Picture : null;

            if (url != null)
            {
                ThumbImageView.StartProgress();
            }

            _imageHelper.ImageUrl = url;
        }

        private void UpdateVisibility()
        {
            ThumbImageView.Hidden = !IsExpanded || 
                !DataContext.HasPicture();

            DetailsLabel.Hidden = !IsExpanded || 
                !DataContext.HasDetailsUrl();

            var isHeaderHidden = !IsExpanded || HeaderView == null;
            HeaderContainer.SetHidden(isHeaderHidden, !isHeaderHidden);

            var isSubHeaderHidden = !IsExpanded || SubHeaderView == null;
            SubHeaderContainer.SetHidden(isSubHeaderHidden, !isSubHeaderHidden);
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
                    GetTextWidth(Frame.Width, DataContext));
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
            StartTimeLabel.Font = Theme.VenueShowCellTimeFont;
            StartTimeLabel.TextColor = Theme.CellText;

            EndTimeLabel.Font = Theme.VenueShowCellTimeFont;
            EndTimeLabel.TextColor = Theme.CellText;

            DescriptionLabel.Font = Theme.VenueShowCellFont;
            DescriptionLabel.TextColor = Theme.CellText;

            DetailsLabel.Font = Theme.VenueShowCellFont;
            DetailsLabel.TextColor = Theme.HyperlinkText;

            HeaderContainer.Layer.ShadowColor = UIColor.Black.CGColor;
            HeaderContainer.Layer.ShadowOffset = new SizeF(0, 2);
            HeaderContainer.Layer.ShadowOpacity = 0.1f;

            SubHeaderContainer.Layer.ShadowColor = UIColor.Black.CGColor;
            SubHeaderContainer.Layer.ShadowOffset = new SizeF(0, 2);
            SubHeaderContainer.Layer.ShadowOpacity = 0.1f;
        }

        private void UpdateClockIcon()
        {
            if (DataContext == null)
            {
                TimeBackgroundView.BackgroundColor = UIColor.Clear;
                return;
            }

            switch (DataContext.GetStatus())
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