using System;
using System.Windows.Input;
using Cirrious.MvvmCross.Binding.Touch.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Core.Model;
using SmartWalk.iOS.Views.Common;

namespace SmartWalk.iOS.Views.OrgEventView
{
    public partial class VenueCell : TableHeaderBase
    {
        public static readonly UINib Nib = UINib.FromName("VenueCell", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("VenueCell");

        private MvxImageViewLoader _imageHelper;
        private UITapGestureRecognizer _cellTapGesture;
        private UITapGestureRecognizer _addressTapGesture;

        public VenueCell(IntPtr handle) : base(handle)
        {
            BackgroundView = new UIView();
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
        }

        protected override void OnDataContextChanged(object previousContext, object newContext)
        {
            LogoImageView.Image = null;

            _imageHelper.ImageUrl = DataContext != null 
                ? DataContext.Info.Logo : null;

            LogoImageView.Hidden = DataContext != null 
                ? DataContext.Info.Logo == null : true;

            NameLeftConstraint.Constant = DataContext != null && 
                DataContext.Info.Logo != null 
                ? 84 : 8;

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
            _cellTapGesture = new UITapGestureRecognizer(() => {
                if (NavigateVenueCommand != null &&
                    NavigateVenueCommand.CanExecute(DataContext))
                {
                    NavigateVenueCommand.Execute(DataContext);
                }
            }) {
                NumberOfTouchesRequired = (uint)1,
                NumberOfTapsRequired = (uint)1
            };

            _cellTapGesture.ShouldReceiveTouch = new UITouchEventArgs((rec, touch) => 
                touch.View != AddressLabel);

            AddGestureRecognizer(_cellTapGesture);

            _addressTapGesture = new UITapGestureRecognizer(() => {
                if (NavigateVenueOnMapCommand != null &&
                    NavigateVenueOnMapCommand.CanExecute(DataContext))
                {
                    NavigateVenueOnMapCommand.Execute(DataContext);
                }
            });

            _addressTapGesture.NumberOfTouchesRequired = (uint)1;
            _addressTapGesture.NumberOfTapsRequired = (uint)1;

            AddressLabel.AddGestureRecognizer(_addressTapGesture);
        }

        private void DisposeGestures()
        {
            if (_cellTapGesture != null)
            {
                RemoveGestureRecognizer(_cellTapGesture);
                _cellTapGesture.Dispose();
                _cellTapGesture = null;
            }
            if (_addressTapGesture != null)
            {
                AddressLabel.RemoveGestureRecognizer(_addressTapGesture);
                _addressTapGesture.Dispose();
                _addressTapGesture = null;
            }
        }
    }
}