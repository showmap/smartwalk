using System;
using System.Windows.Input;
using CoreGraphics;
using Foundation;
using SmartWalk.Client.Core.Model;
using SmartWalk.Client.Core.Utils;
using SmartWalk.Client.iOS.Resources;
using SmartWalk.Client.iOS.Utils;
using SmartWalk.Client.iOS.Views.Common.Base.Cells;
using UIKit;

namespace SmartWalk.Client.iOS.Views.OrgEventView
{
    public partial class VenueHeaderContentView : ContentViewBase
    {
        public static readonly UINib Nib = UINib.FromName("VenueHeaderContentView", NSBundle.MainBundle);

        private UILongPressGestureRecognizer _cellPressGesture;
        private UITapGestureRecognizer _mapTapGesture;

        public VenueHeaderContentView(IntPtr handle) : base(handle)
        {
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
            NameLabel.Text = DataContext.DisplayName();
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

            var initLocation = default(CGPoint);

            _cellPressGesture = new UILongPressGestureRecognizer(rec =>
                {
                    if (rec.State == UIGestureRecognizerState.Began)
                    {
                        SetSelectedState(true);
                        initLocation = rec.LocationInView(
                            UIApplication.SharedApplication.KeyWindow);
                    }
                    else if (rec.State == UIGestureRecognizerState.Changed) {
                        var location = rec.LocationInView(
                            UIApplication.SharedApplication.KeyWindow);
                        var dx = location.X - initLocation.X;
                        var dy = location.Y - initLocation.Y;
                        if (Math.Sqrt(dx * dx + dy * dy) > 15)
                        {
                            SetSelectedState(false);
                            rec.Enabled = false;
                            rec.Enabled = true;
                        }
                    }
                    else if (rec.State == UIGestureRecognizerState.Ended)
                    {
                        SetSelectedState(false);

                        if (rec.LocatedInView(this) &&
                            NavigateVenueCommand != null &&
                            NavigateVenueCommand.CanExecute(DataContext))
                        {
                            NavigateVenueCommand.Execute(DataContext);
                        }
                    }
                }) {
                    MinimumPressDuration = 0.05,
                    Delegate = new TransientGestureDelegate()
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

            NameLabel.Font = Theme.VenueNameFont;
            NameLabel.TextColor = ThemeColors.ContentLightText;

            NavigateOnMapButton.SetImage(ThemeIcons.MapPinSmall, UIControlState.Normal);
            GoRightImageView.Image = ThemeIcons.Forward;
            GoRightImageView.TintColor = ThemeColors.BorderLight;
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
            NavigateOnMapButton.Highlighted = isSelected;
        }
    }

    public class TransientGestureDelegate : UIGestureRecognizerDelegate
    {
        public override bool ShouldRecognizeSimultaneously(
            UIGestureRecognizer gestureRecognizer, 
            UIGestureRecognizer otherGestureRecognizer)
        {
            return true;
        }
    }
}