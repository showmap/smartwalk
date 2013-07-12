using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Cirrious.MvvmCross.Binding.Touch.Views;
using Cirrious.MvvmCross.Binding.Binders;
using SmartWalk.Core.Utils;
using SmartWalk.Core.Model;
using Cirrious.MvvmCross.Binding;

namespace SmartWalk.iOS.Views.Cells
{
    public partial class VenueCell : MvxTableViewCell
    {
        public static readonly UINib Nib = UINib.FromName("VenueCell", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("VenueCell");

        private static MvxBindingDescription[] Bindings = new [] {
            new MvxBindingDescription(
                Reflect<VenueCell>.GetProperty(p => p.NumberText).Name,
                Reflect<Venue>.GetProperty(p => p.Number).Name, 
                null, null, null, MvxBindingMode.OneWay),
            new MvxBindingDescription(
                Reflect<VenueCell>.GetProperty(p => p.NameText).Name,
                ReflectExtensions.GetPath<Venue, EntityInfo>(p => p.Info, p => p.Name), 
                null, null, null, MvxBindingMode.OneWay)
        };

        public VenueCell() : base(Bindings)
        {    
        }

        public VenueCell(IntPtr handle) : base(Bindings, handle)
        {
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
    }
}