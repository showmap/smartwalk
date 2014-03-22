using System;
using System.Drawing;
using System.Windows.Input;
using Cirrious.MvvmCross.Binding.Touch.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Client.Core.Model;
using SmartWalk.Client.iOS.Resources;
using SmartWalk.Client.iOS.Views.Common;

namespace SmartWalk.Client.iOS.Views.OrgEventView
{
    public partial class VenueShowCell : TableCellBase
    {
        public static readonly UINib Nib = UINib.FromName("VenueShowCell", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("VenueShowCell");

        public const int DefaultHeight = Gap + Theme.VenueShowTextLineHeight + Gap;

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
                        ThumbImageView.StopProgress();
                        SetNeedsLayout();
                    }
                    else if (_imageHelper.ImageUrl == null)
                    {
                        ThumbImageView.StopProgress();
                    }
                    else
                    {
                        ThumbImageView.StartProgress();
                    }
                });
        }

        public static VenueShowCell Create()
        {
            return (VenueShowCell)Nib.Instantiate(null, null)[0];
        }

        public static float CalculateCellHeight(float frameWidth, bool isExpanded, VenueShow show)
        {
            if (isExpanded)
            {
                var cellHeight = default(float);

                if (show.Description != null)
                {
                    var logoHeight = show.Logo != null ? Gap + ImageHeight : 0;
                    var detailsHeight = show.Site != null ? Gap + Theme.VenueShowTextLineHeight : 0;
                    var textHeight = CalculateTextHeight(frameWidth - TimeBlockWidth, show.Description);
                    cellHeight = Gap + textHeight + logoHeight + detailsHeight + Gap;
                }

                return cellHeight;
            }

            return DefaultHeight;
        }

        private static int CalculateTextHeight(float frameWidth, string text)
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

                return (int)Math.Ceiling(textSize.Height);
            }

            return 0;
        }

        public ICommand ShowImageFullscreenCommand { get; set; }
        public ICommand ExpandCollapseShowCommand { get; set; }
        public ICommand NavigateDetailsLinkCommand { get; set; }

        public new VenueShow DataContext
        {
            get { return (VenueShow)base.DataContext; }
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
                    SetNeedsUpdateConstraints();
                }
            }
        }

        public bool IsSeparatorVisible
        {
            get
            {
                return !Separator.Hidden;
            }
            set
            {
                Separator.Hidden = !value;
            }
        }

        public override void PrepareForReuse()
        {
            base.PrepareForReuse();

            ThumbImageView.Hidden = true;
            IsExpanded = false;

            UpdateConstraints();
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            ThumbImageView.Hidden = !IsExpanded || 
                DataContext == null || DataContext.Logo == null;

            UpdateConstraints();
        }

        public override void UpdateConstraints()
        {
            base.UpdateConstraints();

            if (IsExpanded &&
                DataContext != null && 
                DataContext.Logo != null &&
                Frame.Height >= CalculateCellHeight(Frame.Width, IsExpanded, DataContext))
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
                DataContext != null && 
                DataContext.Site != null &&
                Frame.Height >= CalculateCellHeight(Frame.Width, IsExpanded, DataContext))
            {
                DetailsHeightConstraint.Constant = Theme.VenueShowTextLineHeight;
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
                    DataContext.Start != DateTime.MinValue
                ? GetTimeText(DataContext.Start, DataContext.Status)
                : new NSAttributedString();

            EndTimeLabel.AttributedText = DataContext != null && 
                    DataContext.End != DateTime.MaxValue
                ? GetTimeText(DataContext.End, DataContext.Status)
                : new NSAttributedString();

            DescriptionLabel.Text = DataContext != null 
                ? DataContext.Description : null;

            UpdateClockIcon();
            UpateImageState();
            SetNeedsUpdateConstraints();
        }

        private static NSAttributedString GetTimeText(DateTime time, VenueShowStatus status)
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
                        Font = status == VenueShowStatus.Finished 
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

        private void UpateImageState()
        {
            _imageHelper.ImageUrl = 
                IsExpanded && DataContext != null 
                    ? DataContext.Logo : null;
        }

        private float GetImageProportionalWidth()
        {
            if (_imageHelper.ImageUrl != null &&
                ThumbImageView.Image != null &&
                IsExpanded)
            {
                var imageSize = ThumbImageView.Image.Size;

                var width = (float)(1.0 * imageSize.Width * ImageHeight / imageSize.Height);
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
                    ShowImageFullscreenCommand.CanExecute(DataContext.Logo))
                {
                    ShowImageFullscreenCommand.Execute(DataContext.Logo);
                }
            }) {
                NumberOfTouchesRequired = (uint)1,
                NumberOfTapsRequired = (uint)1
            };

            _detailsTapGesture = new UITapGestureRecognizer(() => {
                if (NavigateDetailsLinkCommand != null &&
                    NavigateDetailsLinkCommand.CanExecute(DataContext.Site))
                {
                    NavigateDetailsLinkCommand.Execute(DataContext.Site);
                }
            }) {
                NumberOfTouchesRequired = (uint)1,
                NumberOfTapsRequired = (uint)1
            };

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
        }

        private void UpdateClockIcon()
        {
            if (DataContext == null)
            {
                TimeBackgroundView.BackgroundColor = UIColor.Clear;
                return;
            }

            switch (DataContext.Status)
            {
                case VenueShowStatus.NotStarted:
                    TimeBackgroundView.BackgroundColor = UIColor.Clear;
                    break;

                case VenueShowStatus.Started:
                    TimeBackgroundView.BackgroundColor = Theme.HyperlinkText;
                    break;

                case VenueShowStatus.Finished:
                    TimeBackgroundView.BackgroundColor = UIColor.Clear;
                    break;
            }
        }
    }
}