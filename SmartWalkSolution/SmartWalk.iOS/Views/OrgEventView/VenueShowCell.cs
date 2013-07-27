using System;
using System.Drawing;
using System.Linq;
using System.Windows.Input;
using Cirrious.MvvmCross.Binding.BindingContext;
using Cirrious.MvvmCross.Binding.Touch.Views;
using MonoTouch.CoreAnimation;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Core.Converters;
using SmartWalk.Core.Model;
using SmartWalk.iOS.Utils;
using SmartWalk.iOS.Views.Common;

namespace SmartWalk.iOS.Views.OrgEventView
{
    public partial class VenueShowCell : TableCellBase
    {
        private const int LogoHeight = 100;
        private const int DetailsHeight = 18;
        private const int Gap = 8;

        public static readonly UINib Nib = UINib.FromName("VenueShowCell", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("VenueShowCell");

        private readonly MvxImageViewLoader _imageHelper;
        private bool _isTableResizing;
        private bool _isExpanded = true; // all constrains are expanded by default

        public VenueShowCell(IntPtr handle) : base(handle)
        {
            _imageHelper = new MvxImageViewLoader(() => LogoImageView, UpdateLogoImageFrame);

            this.DelayBind(() => {
                var set = this.CreateBindingSet<VenueShowCell, VenueShow>();
                set.Bind().For(p => p.DataContext).To(vs => vs);
                set.Bind(StartTimeLabel).To(vs => vs.Start).WithConversion(new DateTimeFormatConverter(), "t");
                set.Bind(EndTimeLabel).To(vs => vs.End).WithConversion(new DateTimeFormatConverter(), "t");
                set.Bind(DescriptionLabel).To(vs => vs.Description);
                set.Apply();
            });
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
                    UpdateViewState();
                }
            }
        }

        public bool IsTableResizing
        {
            get
            {
                return _isTableResizing;
            }
            set
            {
                if (_isTableResizing != value)
                {
                    _isTableResizing = value;
                    UpdateLogoImageViewState();
                }
            }
        }

        public new VenueShow DataContext
        {
            get
            {
                return (VenueShow)base.DataContext;
            }
            set
            {
                UpdateViewState();
            }
        }

        public override RectangleF Frame
        {
            set
            {
                base.Frame = value;
                UpdateLogoImageFrame();
            }
        }

        public static VenueShowCell Create()
        {
            return (VenueShowCell)Nib.Instantiate(null, null)[0];
        }

        public static float CalculateCellHeight(bool isExpanded, VenueShow show)
        {
            if (isExpanded)
            {
                var textHeight = default(float);

                if (show.Description != null)
                {
                    var logoHeight = show.Logo != null ? Gap + LogoHeight : 0;
                    var detailsHeight = show.Site != null ? Gap + DetailsHeight : 0;
                    var cellWidth = Gap + 60 + 3 + 60 + Gap;
                    var frameSize = new SizeF(ScreenUtil.CurrentScreenWidth - cellWidth, float.MaxValue); 
                    var textSize = new NSString(show.Description).StringSize(
                        UIFont.FromName("Helvetica", 15),
                        frameSize,
                        UILineBreakMode.TailTruncation);
                    textHeight = Gap + textSize.Height + logoHeight + detailsHeight + Gap;
                }

                return textHeight;
            }

            return 35;
        }

        public static void SetVenueCellsTableIsResizing(UITableView tableView, bool isResizing)
        {
            foreach (var cell in tableView.VisibleCells.OfType<VenueShowCell>().ToArray())
            {
                cell.IsTableResizing = isResizing;
            }
        }

        public static void CollapseVenueShowCell(UITableView tableView)
        {
            var cell = tableView.VisibleCells.OfType<VenueShowCell>()
                .FirstOrDefault(c => c.IsExpanded);
            if (cell != null)
            {
                cell.IsExpanded = false;
            }
        }

        public static void ExpandVenueShowCell(UITableView tableView, VenueShow expandedShow)
        {
            var cell = tableView.VisibleCells.OfType<VenueShowCell>()
                .FirstOrDefault(c => Equals(expandedShow, c.DataContext));
            if (cell != null)
            {
                cell.IsExpanded = true;
            }
        }

        public override void PrepareForReuse()
        {
            base.PrepareForReuse();

            DataContext = null;
            IsTableResizing = false;
            LogoImageView.Image = null;
            IsExpanded = false;
        }

        private void UpdateViewState()
        {
            if (DescriptionLogoSpaceConstraint != null)
            {
                DescriptionLogoSpaceConstraint.Constant = 
                    IsExpanded && DataContext != null && DataContext.Logo != null ? Gap : 0;
                LogoHeightConstraint.Constant = 
                    IsExpanded && DataContext != null && DataContext.Logo != null ? LogoHeight : 0;
                MoreInfoHeightConstraint.Constant = 
                    IsExpanded && DataContext != null && DataContext.Site != null ? DetailsHeight : 0;
                LogoDetailsSpaceConstraint.Constant = 
                    IsExpanded && DataContext != null && DataContext.Site != null ? Gap : 0;

                _imageHelper.ImageUrl = IsExpanded && DataContext != null ? DataContext.Logo : null;

                if (!IsExpanded && IsTableResizing)
                {
                    LogoImageView.Image = null;
                    LogoImageView.Hidden = true;
                }
                else if (IsExpanded && !IsTableResizing)
                {
                    LogoImageView.Hidden = false;
                }
            }
        }

        private void UpdateLogoImageViewState()
        {
            if (IsExpanded && 
                DataContext != null && DataContext.Logo != null)
            {
                UpdateLogoImageFrame();

                var transition = new CATransition();
                transition.Duration = 0.25;
                transition.Type = CAAnimation.TransitionFade.ToString();
                LogoImageView.Layer.AddAnimation(transition, null);

                LogoImageView.Hidden = false;
            }
        }

        private void UpdateLogoImageFrame()
        {
            if (_imageHelper.ImageUrl != null &&
                LogoImageView != null && 
                LogoImageView.Image != null && 
                (int)LogoImageView.Layer.Frame.Height != 0)
            {
                var frame = LogoImageView.Layer.Frame;
                var imageSize = LogoImageView.SizeThatFits(frame.Size);

                frame.Size = new SizeF(
                    (float)(1.0 * imageSize.Width * frame.Height / imageSize.Height), 
                    frame.Height);

                LogoImageView.Layer.Frame = frame;
                LayoutIfNeeded();
            }
        }
    }
}