using System;
using System.Linq;
using System.Windows.Input;
using Cirrious.MvvmCross.Binding.Touch.Views;
using MonoTouch.Foundation;
using MonoTouch.MapKit;
using MonoTouch.UIKit;
using SmartWalk.Core.Model;
using SmartWalk.iOS.Resources;
using SmartWalk.iOS.Utils.Map;
using SmartWalk.iOS.Views.Common;

namespace SmartWalk.iOS.Views.OrgEventView
{
    public partial class VenueCell : TableHeaderBase
    {
        public static readonly UINib Nib = UINib.FromName("VenueCell", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("VenueCell");

        public const float DefaultHeight = 63;
        private const float ImageTextGap = 74;
        private const float TextGap = 10;

        private MvxImageViewLoader _imageHelper;
        private UITapGestureRecognizer _cellTapGesture;
        private UILongPressGestureRecognizer _cellPressGesture;

        public VenueCell(IntPtr handle) : base(handle)
        {
            BackgroundView = new UIView { BackgroundColor = Theme.BackgroundPatternColor };
            _imageHelper = new MvxImageViewLoader(() => LogoImageView);
        }

        public new Venue DataContext
        {
            get { return (Venue)base.DataContext; }
            set { base.DataContext = value; }
        }

        public static VenueCell Create()
        {
            return (VenueCell)Nib.Instantiate(null, null)[0];
        }

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

            /*NameLeftConstraint.Constant = DataContext != null && 
                DataContext.Info.Logo != null 
                    ? ImageTextGap : TextGap;*/

            if (DataContext != null && DataContext.Info.Logo == null)
            {
                LogoImageView.Hidden = true;
                MapViewContainer.Hidden = false;

                var annotations = DataContext.Info.Addresses
                    .Select(a => new VenueAnnotation(DataContext, a))
                    .ToArray();
                var coordinates = MapUtil.GetAnnotationsCoordinates(annotations);
                MapView.SetRegion(
                    MapUtil.CoordinateRegionForCoordinates(
                        coordinates,
                        new MKMapSize(5000, 5000)), 
                    false);
            }
            else
            {
                LogoImageView.Hidden = false;
                MapViewContainer.Hidden = true;

                MapView.RemoveAnnotations(MapView.Annotations);
            }

            NameLabel.Text = DataContext != null 
                ? (DataContext.Number == 0 
                    ? DataContext.Info.Name 
                    : string.Format("{0}. {1}", 
                        DataContext.Number, DataContext.Info.Name))
                : null;

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