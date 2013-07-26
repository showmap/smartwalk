using System;
using System.Windows.Input;
using Cirrious.MvvmCross.Binding.BindingContext;
using Cirrious.MvvmCross.Binding.Touch.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Core.Model;
using SmartWalk.iOS.Views.Common;
using SmartWalk.Core.Converters;

namespace SmartWalk.iOS.Views.OrgEventView
{
    public partial class VenueCell : TableCellBase
    {
        public static readonly UINib Nib = UINib.FromName("VenueCell", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("VenueCell");

        private MvxImageViewLoader _imageHelper;

        public VenueCell(IntPtr handle) : base(handle)
        {
            _imageHelper = new MvxImageViewLoader(() => LogoImageView);

            this.DelayBind(() => {
                var set = this.CreateBindingSet<VenueCell, Venue>();
                set.Bind(_imageHelper).To(v => v.Info.Logo);

                set.Bind(LogoImageView).For(p => p.Hidden).To(v => v.Info.Logo)
                    .WithConversion(new ValueConverter<string>(s => s == null), null);

                set.Bind(NameLeftConstraint).For(p => p.Constant).To(v => v.Info.Logo)
                    .WithConversion(new ValueConverter<string>(s => s != null ? 76 : 8), null);

                set.Bind(NameLabel).To(v => v)
                    .WithConversion(new ValueConverter<Venue>(
                        v => v.Number == 0 ? v.Info.Name : string.Format("{0}. {1}", v.Number, v.Info.Name)), null);

                set.Bind(AddressLabel).To(v => v.Info.Addresses)
                    .WithConversion(new ValueConverter<AddressInfo[]>(
                        adds => adds != null && adds.Length > 0 ? adds[0].Address : null), null);

                set.Apply();
            });

            InitializeGesture();
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

        protected override bool Initialize()
        {
            var result = InitializeAddressGesture();
            result = result && InitializeImageView();

            return result;
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

        private bool InitializeAddressGesture()
        {
            if (AddressLabel != null)
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

                return true;
            }

            return false;
        }

        private bool InitializeImageView()
        {
            if (LogoImageView != null)
            {
                LogoImageView.BackgroundColor = UIColor.White;
                LogoImageView.ClipsToBounds = true;
                LogoImageView.Layer.BorderColor = UIColor.LightGray.CGColor;
                LogoImageView.Layer.BorderWidth = 1;
                LogoImageView.Layer.CornerRadius = 5;

                return true;
            }

            return false;
        }
    }
}