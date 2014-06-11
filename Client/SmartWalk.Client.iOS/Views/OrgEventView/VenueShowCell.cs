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
using SmartWalk.Client.iOS.Views.Common.Base;
using SmartWalk.Client.iOS.Utils;

namespace SmartWalk.Client.iOS.Views.OrgEventView
{
    public partial class VenueShowCell : TableCellBase
    {
        public static readonly UINib Nib = UINib.FromName("VenueShowCell", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("VenueShowCell");

        public readonly static float DefaultHeight = Gap + 
            (float)Math.Ceiling(Theme.VenueShowCellFont.LineHeight) + Gap;

        private static readonly string TimeFormat = "{0:t}";
        private static readonly string Space = " ";
        private static readonly char M = 'm';

        private const int ImageHeight = 100;
        private const int TimeBlockWidth = 109;
        private const int Gap = 12;

        private readonly MvxImageViewLoader _imageHelper;
        private UITapGestureRecognizer _imageTapGesture;
        private UITapGestureRecognizer _cellTapGesture;
        private UITapGestureRecognizer _detailsTapGesture;
        private bool _isExpanded;
        private UIView _headerView;

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
            Show show)
        {
            if (isExpanded)
            {
                var cellHeight = isHeaderVisible 
                    ? VenueHeaderView.DefaultHeight 
                    : 0f;

                var showText = show.GetText();  
                if (showText != null)
                {
                    var logoHeight = show.HasPicture() ? Gap + ImageHeight : 0;
                    var detailsHeight = show.HasDetailsUrl()
                        ? Gap + (float)Math.Ceiling(Theme.VenueShowCellFont.LineHeight) 
                        : 0;
                    var textHeight = 
                        (float)Math.Ceiling(CalculateTextHeight(GetTextWidth(frameWidth), showText));
                    cellHeight += Math.Max(
                        DefaultHeight, 
                        Gap + textHeight + logoHeight + detailsHeight + Gap); // if no text, we still show times
                }

                return cellHeight;
            }

            return DefaultHeight;
        }

        private static float CalculateTextHeight(float frameWidth, string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                var frameSize = new SizeF(frameWidth, float.MaxValue); 
                SizeF textSize;

                // TODO: to complete iOS7 text measuring some day
                using (var ns = new NSString(text))
                {
                    textSize = ns.StringSize(
                        Theme.VenueShowCellFont,
                        frameSize,
                        UILineBreakMode.TailTruncation);
                }

                return textSize.Height;
            }

            return 0;
        }

        private static float GetTextWidth(float frameWidth)
        {
            return frameWidth - TimeBlockWidth - 8; // Description Label right gap
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
                        DataContext)
                    : 0;

            HeaderHeightConstraint.Constant = 
                IsExpanded && 
                HeaderView != null &&
                Frame.Height >= calculatedHeight
                    ? VenueHeaderView.DefaultHeight
                    : 0;

            DescriptionLeftConstraint.Constant = TimeBlockWidth;

            if (IsExpanded &&
                DataContext.HasPicture() &&
                Frame.Height >= calculatedHeight)
            {
                ImageHeightConstraint.Constant = ImageHeight;
                ImageWidthConstraint.Constant = GetImageProportionalWidth();
                DescriptionAndImageSpaceConstraint.Constant = Gap;
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
                ImageAndDetailsSpaceConstraint.Constant = Gap;
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

            StartTimeLabel.AttributedText = DataContext != null && 
                    DataContext.StartTime.HasValue
                ? GetTimeText(DataContext.StartTime.Value, DataContext.GetStatus())
                : new NSAttributedString();

            EndTimeLabel.AttributedText = DataContext != null && 
                    DataContext.EndTime.HasValue
                ? GetTimeText(DataContext.EndTime.Value, DataContext.GetStatus())
                : new NSAttributedString();

            AllDayLabel.Text = DataContext != null &&
                    !DataContext.StartTime.HasValue &&
                    !DataContext.EndTime.HasValue
                ? Localization.AllDay
                : null;

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
                        Font = Theme.VenueShowCellFont,
                        ForegroundColor = Theme.CellTextPassive
                    },
                    new NSRange(
                        show.Title.Length, 
                        result.Length - show.Title.Length));
            }

            return result;
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
                    GetTextWidth(Frame.Width));
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

            AllDayLabel.Font = Theme.VenueShowCellTimeFont;
            AllDayLabel.TextColor = Theme.CellText;

            DescriptionLabel.Font = Theme.VenueShowCellFont;
            DescriptionLabel.TextColor = Theme.CellText;

            DetailsLabel.Font = Theme.VenueShowCellFont;
            DetailsLabel.TextColor = Theme.HyperlinkText;

            HeaderContainer.Layer.ShadowColor = UIColor.Black.CGColor;
            HeaderContainer.Layer.ShadowOffset = new SizeF(0, 2);
            HeaderContainer.Layer.ShadowOpacity = 0.1f;
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
                HeaderView != null 
                    ? Theme.CellSemiHighlight 
                    : Theme.CellBackground;
        }
    }
}