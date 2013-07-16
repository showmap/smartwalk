using System;
using System.Windows.Input;
using Cirrious.MvvmCross.Binding;
using Cirrious.MvvmCross.Binding.Binders;
using Cirrious.MvvmCross.Binding.Touch.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Core.Converters;
using SmartWalk.Core.Model;
using SmartWalk.Core.Utils;

namespace SmartWalk.iOS.Views.Cells
{
    public partial class VenueCell : MvxTableViewCell
    {
        public static readonly UINib Nib = UINib.FromName("VenueCell", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("VenueCell");

        private static readonly MvxBindingDescription[] Bindings = new [] {
            new MvxBindingDescription(
                Reflect<VenueCell>.GetProperty(p => p.NumberText).Name,
                Reflect<Venue>.GetProperty(p => p.Number).Name, 
                null, null, null, MvxBindingMode.OneWay),
            new MvxBindingDescription(
                Reflect<VenueCell>.GetProperty(p => p.NameText).Name,
                ReflectExtensions.GetPath<Venue, EntityInfo>(p => p.Info, p => p.Name), 
                null, null, null, MvxBindingMode.OneWay),
            new MvxBindingDescription(
                Reflect<VenueCell>.GetProperty(p => p.AddressText).Name,
                ReflectExtensions.GetPath<Venue, EntityInfo>(p => p.Info, p => p.Addresses), 
                new AddressesConverter(), null, null, MvxBindingMode.OneWay)
        };

        public VenueCell() : base(Bindings)
        {
            InitGesture();
        }

        public VenueCell(IntPtr handle) : base(Bindings, handle)
        {
            InitGesture();
        }

        public static VenueCell Create()
        {
            return (VenueCell)Nib.Instantiate(null, null)[0];
        }

        public string NumberText {
            get { return NumberLabel.Text; }
            set { NumberLabel.Text = value; }
        }

        public string NameText {
            get { return NameLabel.Text; }
            set { NameLabel.Text = value; }
        }

        public string AddressText {
            get { return AddressLabel.Text; }
            set { AddressLabel.Text = value; }
        }

        public ICommand NavigateVenueCommand { get; set; }

        public ICommand NavigateVenueOnMapCommand { get; set; }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            InitAddressGesture();
        }

        private void InitGesture()
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

        private void InitAddressGesture()
        {
            if (AddressLabel != null &&
                (AddressLabel.GestureRecognizers == null ||
                AddressLabel.GestureRecognizers.Length == 0))
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
    }
}