using System;
using System.Windows.Input;
using Cirrious.MvvmCross.Binding.Touch.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Core.Model;
using SmartWalk.iOS.Views.Common;

namespace SmartWalk.iOS.Views.OrgEventView
{
    public partial class VenueCell : TableCellBase<Venue>
    {
        public static readonly UINib Nib = UINib.FromName("VenueCell", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("VenueCell");

        private MvxImageViewLoader _imageHelper;
        private Venue _dataContext;

        public VenueCell(IntPtr handle) : base(handle)
        {
            _imageHelper = new MvxImageViewLoader(() => LogoImageView);
        }

        public static VenueCell Create()
        {
            return (VenueCell)Nib.Instantiate(null, null)[0];
        }

        public ICommand NavigateVenueCommand { get; set; }

        public ICommand NavigateVenueOnMapCommand { get; set; }

        public override void PrepareForReuse()
        {
            base.PrepareForReuse();

            LogoImageView.Image = null;
        }

        protected override void OnInitialize()
        {
            InitializeGesture();
            InitializeAddressGesture();
            InitializeImageView();
        }

        protected override void OnDataContextChanged()
        {
            _imageHelper.ImageUrl = DataContext != null 
                ? DataContext.Info.Logo : null;

            LogoImageView.Hidden = DataContext != null 
                ? DataContext.Info.Logo == null : true;

            NameLeftConstraint.Constant = DataContext != null && 
                DataContext.Info.Logo != null 
                ? 76 : 8;

            NameLabel.Text = DataContext != null 
                ? (DataContext.Number == 0 
                    ? DataContext.Info.Name 
                    : string.Format("{0}. {1}", 
                        DataContext.Number, DataContext.Info.Name))
                : null;

            AddressLabel.Text = DataContext != null && 
                    DataContext.Info.Addresses != null && 
                    DataContext.Info.Addresses.Length > 0 
                ? DataContext.Info.Addresses[0].Address 
                : null;
        }

        private void InitializeGesture()
        {
            var tap = new UITapGestureRecognizer(() => {
                if (NavigateVenueCommand != null &&
                    NavigateVenueCommand.CanExecute(DataContext))
                {
                    NavigateVenueCommand.Execute(DataContext);
                }
            });

            tap.NumberOfTouchesRequired = (uint)1;
            tap.NumberOfTapsRequired = (uint)1;

            tap.ShouldReceiveTouch = new UITouchEventArgs((rec, touch) => 
                touch.View != AddressLabel);

            AddGestureRecognizer(tap);
        }

        private void InitializeAddressGesture()
        {
            if (AddressLabel.GestureRecognizers == null ||
                AddressLabel.GestureRecognizers.Length == 0)
            {
                var tap = new UITapGestureRecognizer(() => {
                    if (NavigateVenueOnMapCommand != null &&
                        NavigateVenueOnMapCommand.CanExecute(DataContext))
                    {
                        NavigateVenueOnMapCommand.Execute(DataContext);
                    }
                });

                tap.NumberOfTouchesRequired = (uint)1;
                tap.NumberOfTapsRequired = (uint)1;

                AddressLabel.AddGestureRecognizer(tap);
            }
        }

        private void InitializeImageView()
        {
            LogoImageView.BackgroundColor = UIColor.White;
            LogoImageView.ClipsToBounds = true;
            //LogoImageView.Layer.BorderColor = UIColor.LightGray.CGColor;
            //LogoImageView.Layer.BorderWidth = 1;
            LogoImageView.Layer.CornerRadius = 3;
        }
    }
}