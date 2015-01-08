using System;
using System.Drawing;
using System.Windows.Input;
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

        private UILongPressGestureRecognizer _cellPressGesture;
        private UITapGestureRecognizer _mapTapGesture;

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
            _mapTapGesture = new UITapGestureRecognizer(() =>
                {
                    if (NavigateVenueOnMapCommand != null &&
                        NavigateVenueOnMapCommand.CanExecute(DataContext))
                    {
                        NavigateVenueOnMapCommand.Execute(DataContext);
                    }
                });

            NavigateOnMapButton.AddGestureRecognizer(_mapTapGesture);

            _cellPressGesture = new UILongPressGestureRecognizer(rec =>
                {
                    if (rec.State == UIGestureRecognizerState.Began)
                    {
                        SetSelectedState(true);
                    }
                    else if (rec.State == UIGestureRecognizerState.Ended)
                    {
                        SetSelectedState(false);

                        if (Frame.IntersectsWith(new RectangleF(rec.LocationInView(this), SizeF.Empty)) &&
                            NavigateVenueCommand != null &&
                            NavigateVenueCommand.CanExecute(DataContext))
                        {
                            NavigateVenueCommand.Execute(DataContext);
                        }
                    }
                }) {
                    MinimumPressDuration = 0.05
                };
            _cellPressGesture.RequireGestureRecognizerToFail(_mapTapGesture);

            AddGestureRecognizer(_cellPressGesture);
        }

        private void DisposeGestures()
        {
            if (_mapTapGesture != null)
            {
                NavigateOnMapButton.RemoveGestureRecognizer(_mapTapGesture);
                _mapTapGesture.Dispose();
                _mapTapGesture = null;
            }

            if (_cellPressGesture != null)
            {
                RemoveGestureRecognizer(_cellPressGesture);
                _cellPressGesture.Dispose();
                _cellPressGesture = null;
            }
        }

        private void InitializeStyle()
        {
            BottomSeparator.IsLineOnTop = true;

            ImageLabel.Font = Theme.VenueThumbLabelFont;
            ImageLabel.TextColor = ThemeColors.ContentDarkText;
            ImageLabelView.BackgroundColor = ThemeColors.BorderLight;

            NameLabel.Font = Theme.ContentFont;
            NameLabel.TextColor = ThemeColors.ContentLightText;

            AddressLabel.Font = Theme.VenueAddressFont;
            AddressLabel.TextColor = ThemeColors.ContentLightTextPassive;

            NavigateOnMapButton.SetImage(ThemeIcons.MapPinSmall, UIControlState.Normal);
            GoRightImageView.Image = ThemeIcons.Forward;
            GoRightImageView.TintColor = ThemeColors.BorderDark.ColorWithAlpha(0.9f);
        }

        private void SetSelectedState(bool isSelected)
        {
            if (isSelected)
            {
                BackgroundView.BackgroundColor = ThemeColors.ContentLightHighlight;
            }
            else
            {
                BackgroundView.BackgroundColor = ThemeColors.PanelBackgroundAlpha;
            }

            NameLabel.Highlighted = isSelected;
            AddressLabel.Highlighted = isSelected;
            NavigateOnMapButton.Highlighted = isSelected;
        }
    }
}