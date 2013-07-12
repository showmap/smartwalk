using System;
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
    public partial class VenueShowCell : MvxTableViewCell
    {
        public static readonly UINib Nib = UINib.FromName("VenueShowCell", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("VenueShowCell");

        private static MvxBindingDescription[] Bindings = new [] {
            new MvxBindingDescription(
                Reflect<VenueShowCell>.GetProperty(p => p.StartTimeText).Name,
                Reflect<VenueShow>.GetProperty(p => p.Start).Name, 
                new DateTimeFormatConverter(), "t", 
                null, MvxBindingMode.OneWay),
            new MvxBindingDescription(
                Reflect<VenueShowCell>.GetProperty(p => p.EndTimeText).Name,
                Reflect<VenueShow>.GetProperty(p => p.End).Name, 
                new DateTimeFormatConverter(), "t", 
                null, MvxBindingMode.OneWay),
            new MvxBindingDescription(
                Reflect<VenueShowCell>.GetProperty(p => p.DescriptionText).Name,
                Reflect<VenueShow>.GetProperty(p => p.Description).Name, 
                null, null, null, MvxBindingMode.OneWay)
        };

        public VenueShowCell() : base(Bindings)
        {    
        }

        public VenueShowCell(IntPtr handle) : base(Bindings, handle)
        {
        }

        public static VenueShowCell Create()
        {
            return (VenueShowCell)Nib.Instantiate(null, null)[0];
        }

        public string StartTimeText {
            get { return StartTimeLabel.Text; }
            set { StartTimeLabel.Text = value; }
        }

        public string EndTimeText {
            get { return EndTimeLabel.Text; }
            set { EndTimeLabel.Text = value; }
        }

        public string DescriptionText {
            get { return DescriptionLabel.Text; }
            set { DescriptionLabel.Text = value; }
        }
    }
}