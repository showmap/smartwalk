using System;
using System.Drawing;
using System.Windows.Input;
using MonoTouch.CoreAnimation;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Client.Core.Model;
using SmartWalk.Client.Core.Utils;
using SmartWalk.Shared.Utils;
using SmartWalk.Client.iOS.Resources;
using SmartWalk.Client.iOS.Utils;
using SmartWalk.Client.iOS.Utils.MvvmCross;
using SmartWalk.Client.iOS.Views.Common.Base.Cells;

namespace SmartWalk.Client.iOS.Views.OrgEventView
{
    public partial class VenueHeaderContentView : ContentViewBase
    {
        public static readonly UINib Nib = UINib.FromName("VenueHeaderContentView", NSBundle.MainBundle);

        private readonly MvxResizedImageViewLoader _imageHelper;
        private readonly AnimationDelay _animationDelay = new AnimationDelay();

        private UITapGestureRecognizer _cellTapGesture;
        private UILongPressGestureRecognizer _cellPressGesture;

        public VenueHeaderContentView(IntPtr handle) : base(handle)
        {
            _imageHelper = new MvxResizedImageViewLoader(
                () => LogoImageView,
                () => 
                {
                    if (LogoImageView != null && LogoImageView.ProgressEnded())
                    {
                        var noImage = !LogoImageView.HasImage();
                        LogoImageView.SetHidden(noImage, _animationDelay.Animate);

                        // showing abbr if image couldn't be loaded
                        ImageLabelView.SetHidden(!noImage, false);
                    }
                });
        }

        public static VenueHeaderContentView Create()
        {
            return (VenueHeaderContentView)Nib.Instantiate(null, null)[0];
        }

        public new Venue DataContext
        {
            get { return (Venue)base.DataContext; }
            set { base.DataContext = value; }
        }

        public UIView BackgroundView { get;set; }

        public ICommand NavigateVenueCommand { get; set; }
        public ICommand NavigateVenueOnMapCommand { get; set; }

        public override void WillMoveToSuperview(UIView newsuper)
        {
            base.WillMoveToSuperview(newsuper);

            if (newsuper == null)
            {
                NavigateVenueCommand = null;
                NavigateVenueOnMapCommand = null;

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

            LogoImageView.Image = null;
            _imageHelper.ImageUrl = null;

            if (DataContext != null)
            {
                if (DataContext.Info.HasPicture())
                {
                    LogoImageView.Hidden = false;
                    ImageLabelView.Hidden = true;
                }
                else
                {
                    LogoImageView.Hidden = true;
                    ImageLabelView.Hidden = false;
                }
            }

            NameLabel.Text = DataContext.DisplayName();
            ImageLabel.Text = DataContext.Info.Name.GetAbbreviation(2);

            AddressLabel.Text = DataContext != null
                ? DataContext.Info.GetAddressText()
                : null;

            _imageHelper.ImageUrl = DataContext != null 
                ? DataContext.Info.Picture : null;

            UpdateConstraintConstants();
        }

        private void UpdateConstraintConstants()
        {
            if (DataContext != null && DataContext.Info.HasAddressText())
            {
                TitleTopGapConstraint.Constant = 12;
                TitleLeftGapConstraint.Constant = 8;
                PinTopGapConstraint.Constant = 22;
            }
            else
            {
                TitleTopGapConstraint.Constant = 23;
                TitleLeftGapConstraint.Constant = 22;
                PinTopGapConstraint.Constant = 10;
            }
        }

        private void InitializeGestures()
        {
            var selectedAction = new NSAction(() => 
                {
                    SetSelectedState(true);

                    if (NavigateVenueCommand != null &&
                        NavigateVenueCommand.CanExecute(DataContext))
                    {
                        NavigateVenueCommand.Execute(DataContext);
                    }

                    NSTimer.CreateScheduledTimer(
                        TimeSpan.MinValue,
                        () => SetSelectedState(false));
                });

            // TODO: fail if it's ended outside of the cell
            _cellPressGesture = new UILongPressGestureRecognizer(rec => 
                {
                    if (rec.State == UIGestureRecognizerState.Began)
                    {
                        SetSelectedState(true);
                    }
                    else if (rec.State == UIGestureRecognizerState.Ended)
                    {
                        selectedAction();
                    }
                });

            _cellTapGesture = new UITapGestureRecognizer(selectedAction);

            AddGestureRecognizer(_cellTapGesture);
            AddGestureRecognizer(_cellPressGesture);
        }

        private void DisposeGestures()
        {
            if (_cellPressGesture != null)
            {
                RemoveGestureRecognizer(_cellPressGesture);
                _cellPressGesture.Dispose();
                _cellPressGesture = null;
            }

            if (_cellTapGesture != null)
            {
                RemoveGestureRecognizer(_cellTapGesture);
                _cellTapGesture.Dispose();
                _cellTapGesture = null;
            }
        }

        private void InitializeStyle()
        {
            BottomSeparator.IsLineOnTop = true;

            ImageLabel.Font = Theme.VenueCellThumbLabelFont;
            ImageLabel.TextColor = Theme.VenueCellThumbLabel;
            ImageLabelView.BackgroundColor = Theme.VenueCellThumb;

            NameLabel.Font = Theme.VenueCellTitleFont;
            NameLabel.TextColor = Theme.CellText;

            AddressLabel.Font = Theme.VenueCellAddressFont;
            AddressLabel.TextColor = Theme.CellTextPassive;

            NavigateOnMapButton.SetImage(ThemeIcons.MapPinSmall, UIControlState.Normal);
            GoRightImageView.Image = ThemeIcons.Forward;
            GoRightImageView.TintColor = Theme.IconVeryPassive;
        }

        partial void OnNavigateOnMapClick(UIButton sender)
        {
            if (NavigateVenueOnMapCommand != null &&
                NavigateVenueOnMapCommand.CanExecute(DataContext))
            {
                NavigateVenueOnMapCommand.Execute(DataContext);
            }
        }

        private void SetSelectedState(bool isSelected)
        {
            if (isSelected)
            {
                BackgroundView.BackgroundColor = Theme.CellHighlight;
            }
            else
            {
                BackgroundView.BackgroundColor = Theme.HeaderCellBackground;
            }

            NameLabel.Highlighted = isSelected;
            AddressLabel.Highlighted = isSelected;
            NavigateOnMapButton.Highlighted = isSelected;
        }
    }
}