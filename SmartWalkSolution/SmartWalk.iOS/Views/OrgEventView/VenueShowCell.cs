using System;
using System.Linq;
using System.Drawing;
using Cirrious.MvvmCross.Binding.Touch.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Core.Model;
using SmartWalk.iOS.Views.Common;
using MonoTouch.CoreGraphics;
using MonoTouch.CoreAnimation;

namespace SmartWalk.iOS.Views.OrgEventView
{
    public partial class VenueShowCell : TableCellBase
    {
        public const int DefaultCellHeight = Gap + TextLineHeight + Gap;

        public static readonly UINib Nib = UINib.FromName("VenueShowCell", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("VenueShowCell");

        private const int ImageHeight = 100;
        private const int TimeLabelWidth = 60;
        private const int TextLineHeight = 19;
        private const int Gap = 8;

        private readonly MvxImageViewLoader _imageHelper;
        private bool _isExpanded;

        public VenueShowCell(IntPtr handle) : base(handle)
        {
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
                    var detailsHeight = show.Site != null ? Gap + TextLineHeight : 0;
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
                    SetNeedsUpdateConstraints();
                }
            }
        }

        public override void PrepareForReuse()
        {
            base.PrepareForReuse();

            ThumbImageView.Image = null;
            ThumbImageView.Hidden = true;
            IsExpanded = false;

            UpdateConstraints();
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            ThumbImageView.Hidden = !IsExpanded || DataContext == null || DataContext.Logo == null;

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
                DetailsHeightConstraint.Constant = TextLineHeight;
                ImageAndDetailsSpaceConstraint.Constant = Gap - 3;
            }
            else
            {
                DetailsHeightConstraint.Constant = 0;
                ImageAndDetailsSpaceConstraint.Constant = 0;
            }

            EndTimeLeftSpaceConstraint.Constant =
                EndTimeLabel.Text != null &&
                    EndTimeLabel.Text.StartsWith("1", StringComparison.InvariantCultureIgnoreCase)
                    ? 2 : 3;
            EndTimeRightSpaceConstraint.Constant =
                EndTimeLabel.Text != null &&
                    EndTimeLabel.Text.StartsWith("1", StringComparison.InvariantCultureIgnoreCase)
                    ? 9 : 8;
        }

        protected override void OnDataContextChanged(object previousContext, object newContext)
        {
            var timeTextColor = 
                DataContext == null ||
                (DataContext.Start.Date != DateTime.Now.Date) ||
                (DataContext.Start == DateTime.MinValue && DataContext.End >= DateTime.Now) ||
                (DataContext.End == DateTime.MaxValue) ||
                (DataContext.End >= DateTime.Now)
                    ? UIColor.Black : UIColor.LightGray;

            if (DataContext != null &&
                DataContext.Start.Date == DateTime.Now.Date &&
                DataContext.Start <= DateTime.Now &&
                DateTime.Now <= DataContext.End)
            {
                timeTextColor = UIColor.FromRGB(9, 135, 16);
            }

            StartTimeLabel.Text = DataContext != null && DataContext.Start != DateTime.MinValue
                ? String.Format("{0:t}", DataContext.Start) : null;
            StartTimeLabel.TextColor = timeTextColor;

            EndTimeLabel.Text = DataContext != null && DataContext.End != DateTime.MaxValue
                ? String.Format("{0:t}", DataContext.End) : null;
            EndTimeLabel.TextColor = timeTextColor;

            DescriptionLabel.Text = DataContext != null 
                ? DataContext.Description : null;

            UpateImageState();
            SetNeedsUpdateConstraints();
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
    }
}