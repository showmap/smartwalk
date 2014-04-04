﻿using System;
using System.Linq;
using System.Windows.Input;
using Cirrious.MvvmCross.Binding.Touch.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Client.Core.Model;
using SmartWalk.Client.iOS.Resources;
using SmartWalk.Client.iOS.Views.Common;

namespace SmartWalk.Client.iOS.Views.OrgEventView
{
    public partial class VenueHeaderContentView : ContentViewBase
    {
        public static readonly UINib Nib = UINib.FromName("VenueHeaderContentView", NSBundle.MainBundle);

        private MvxImageViewLoader _imageHelper;
        private UITapGestureRecognizer _cellTapGesture;
        private UILongPressGestureRecognizer _cellPressGesture;

        public VenueHeaderContentView(IntPtr handle) : base(handle)
        {
            _imageHelper = new MvxImageViewLoader(() => LogoImageView);
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
            LogoImageView.Image = null;
            _imageHelper.ImageUrl = null;

            _imageHelper.ImageUrl = DataContext != null 
                ? DataContext.Info.Logo : null;

            if (DataContext != null && DataContext.Info.Logo == null)
            {
                LogoImageView.Hidden = true;
                ImageLabelView.Hidden = false;

                ImageLabel.Text = DataContext.Info.Name != null 
                    ? DataContext.Info.Name.FirstOrDefault().ToString()
                    : null;
            }
            else
            {
                LogoImageView.Hidden = false;
                ImageLabelView.Hidden = true;
            }

            NameLabel.Text = DataContext.Info.Name;

            // TODO: to support showing more than one address
            AddressLabel.Text = DataContext != null && 
                DataContext.Info.Addresses != null && 
                DataContext.Info.Addresses.Length > 0 
                ? DataContext.Info.Addresses[0].Address 
                : null;
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

            NameLabel.Font = Theme.VenueCellTitleFont;
            NameLabel.TextColor = Theme.CellText;

            AddressLabel.Font = Theme.VenueCellAddressFont;
            AddressLabel.TextColor = Theme.CellTextPassive;

            NavigateOnMapButton.SetImage(ThemeIcons.SmallMap, UIControlState.Normal);
            GoRightImageView.Image = ThemeIcons.GoRight;
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
                BackgroundView.BackgroundColor = Theme.BackgroundPatternColor;
            }

            NameLabel.Highlighted = isSelected;
            AddressLabel.Highlighted = isSelected;
            NavigateOnMapButton.Highlighted = isSelected;
        }
    }
}