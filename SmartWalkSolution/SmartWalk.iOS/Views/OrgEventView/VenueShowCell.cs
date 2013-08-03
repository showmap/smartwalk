using System;
using System.Drawing;
using Cirrious.MvvmCross.Binding.Touch.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Core.Model;
using SmartWalk.iOS.Controls;
using SmartWalk.iOS.Views.Common;

namespace SmartWalk.iOS.Views.OrgEventView
{
    public class VenueShowCell : TableCellBase
    {
        public const int DefaultCellHeight = Gap + TextLineHeight + Gap;

        public static readonly NSString Key = new NSString("VenueShowCell");

        private const int ImageHeight = 100;
        private const int TimeLabelWidth = 60;
        private const int TextLineHeight = 19;
        private const int Gap = 8;

        private readonly MvxImageViewLoader _imageHelper;
        private UILabel _startLabel;
        private UILabel _endLabel;
        private UILabel _descriptionLabel;
        private UIImageView _imageView;
        private UILabel _detailsLabel;

        private bool _isExpanded;

        public VenueShowCell(IntPtr handle) : base(handle)
        {
            Initialize();

            _imageHelper = new MvxImageViewLoader(
                () => _imageView, 
                () => {
                    if (_imageHelper.ImageUrl != null && _imageView.Image != null)
                    {
                        SetNeedsLayout();
                    }
                });
        }

        public static float CalculateCellHeight(float frameWidth, bool isExpanded, VenueShow show)
        {
            if (isExpanded)
            {
                var cellHeight = default(float);

                if (show.Description != null)
                {
                    var logoHeight = show.Logo != null ? Gap + ImageHeight : 0;
                    var detailsHeight = show.Site != null ? TextLineHeight : 0;
                    var timeLabelsWidth = Gap + TimeLabelWidth + 3 + TimeLabelWidth + Gap;
                    var textHeight = CalculateTextHeight(frameWidth - timeLabelsWidth, show.Description);
                    cellHeight = Gap + textHeight + logoHeight + detailsHeight + Gap;
                }

                return cellHeight;
            }

            return DefaultCellHeight;
        }

        private static float CalculateTextHeight(float frameWidth, string text)
        {
            if (text != null && text != string.Empty)
            {
                var frameSize = new SizeF(frameWidth, float.MaxValue); 
                var textSize = new NSString(text).StringSize(
                    UIFont.FromName("Helvetica", 15),
                    frameSize,
                    UILineBreakMode.TailTruncation);

                return textSize.Height;
            }

            return 0;
        }

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
                }
            }
        }

        public override void PrepareForReuse()
        {
            base.PrepareForReuse();

            _imageView.Image = null;
            _imageView.Hidden = true;
            IsExpanded = false;
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            _startLabel.Frame = new RectangleF(Gap, 7, TimeLabelWidth, TextLineHeight);
            _endLabel.Frame = new RectangleF(Gap + TimeLabelWidth + 3, 7, TimeLabelWidth, TextLineHeight);

            var timeRectWidth = Gap + TimeLabelWidth + 3 + TimeLabelWidth + Gap;
            var textWidth = Frame.Width - timeRectWidth - Gap;
            var textHeight = IsExpanded 
                ? CalculateTextHeight(textWidth, _descriptionLabel.Text) 
                : TextLineHeight;

            _descriptionLabel.Frame = new RectangleF(timeRectWidth, Gap, textWidth, textHeight);

            var detailsHeight = DataContext != null && 
                DataContext.Site != null && IsExpanded ? TextLineHeight : 0;

            _detailsLabel.Frame = new RectangleF(
                timeRectWidth,
                Frame.Height - detailsHeight - Gap,
                textWidth,
                detailsHeight);

            var imageWidth = GetImageWidth();
            _imageView.Frame = new RectangleF(
                timeRectWidth,
                Gap + textHeight + Gap,
                imageWidth,
                Math.Min(ImageHeight, Frame.Height - Gap - textHeight - detailsHeight - Gap));

            _imageView.Hidden = !IsExpanded && _imageView.Frame.Height < ImageHeight - 5;
        }

        protected override void OnDataContextChanged(object previousContext, object newContext)
        {
            _startLabel.Text = DataContext != null 
                ? String.Format("{0:t}", DataContext.Start) : null;
            _endLabel.Text = DataContext != null 
                ? String.Format("{0:t}", DataContext.End) : null;
            _descriptionLabel.Text = DataContext != null 
                ? DataContext.Description : null;

            UpateImageState();
            SetNeedsLayout();

            // TODO: to adjust times that start with 1 digit
            /*EndTimeLeftSpaceConstraint.Constant =
                EndTimeLabel.Text != null &&
                EndTimeLabel.Text.StartsWith("1")
                    ? 2 : 3;
            EndTimeRightSpaceConstraint.Constant =
                EndTimeLabel.Text != null &&
                    EndTimeLabel.Text.StartsWith("1")
                    ? 9 : 8;*/
        }

        private void Initialize()
        {
            SelectionStyle = UITableViewCellSelectionStyle.None;

            // TIME

            var font = UIFont.BoldSystemFontOfSize(13);
            _startLabel = new UILabel { Font = font };
            _endLabel = new UILabel { Font = font };

            AddSubview(_startLabel);
            AddSubview(_endLabel);

            // DESCRIPTION

            font = UIFont.SystemFontOfSize(15);
            _descriptionLabel = new UILabel { 
                Font = font, 
                Lines = 0, 
                LineBreakMode = UILineBreakMode.TailTruncation,
                ContentMode = UIViewContentMode.Top
            };

            AddSubview(_descriptionLabel);

            // IMAGE

            _imageView = new UIImageView { 
                ContentMode = UIViewContentMode.ScaleAspectFit,
                ClipsToBounds = true,
                Hidden = true
            };
 
            AddSubview(_imageView);

            // MORE INFO

            font = UIFont.SystemFontOfSize(13);
            _detailsLabel = new UILabel { 
                Font = font, 
                TextColor = ThemeColors.Aqua,
                Text = "more info"
            };

            AddSubview(_detailsLabel);
        }

        private void UpateImageState()
        {
            _imageHelper.ImageUrl = 
                IsExpanded && DataContext != null 
                    ? DataContext.Logo : null;
        }

        private float GetImageWidth()
        {
            if (_imageHelper.ImageUrl != null &&
                _imageView.Image != null &&
                IsExpanded)
            {
                var imageSize = _imageView.Image.Size;

                var width = (float)(1.0 * imageSize.Width * ImageHeight / imageSize.Height);
                return width;
            }

            return 0;
        }

    }
}