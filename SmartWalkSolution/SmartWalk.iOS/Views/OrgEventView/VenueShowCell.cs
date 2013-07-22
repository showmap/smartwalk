using System;
using Cirrious.MvvmCross.Binding.BindingContext;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Core.Converters;
using SmartWalk.Core.Model;
using SmartWalk.iOS.Views.Common;

namespace SmartWalk.iOS.Views.OrgEventView
{
    public partial class VenueShowCell : TableCellBase
    {
        public static readonly UINib Nib = UINib.FromName("VenueShowCell", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("VenueShowCell");

        public VenueShowCell(IntPtr handle) : base(handle)
        {
            this.DelayBind(() => {
                var set = this.CreateBindingSet<VenueShowCell, VenueShow>();
                set.Bind(StartTimeLabel).To(vs => vs.Start).WithConversion(new DateTimeFormatConverter(), "t");
                set.Bind(EndTimeLabel).To(vs => vs.End).WithConversion(new DateTimeFormatConverter(), "t");
                set.Bind(DescriptionLabel).To(vs => vs.Description);
                set.Apply();
            });
        }

        public static VenueShowCell Create()
        {
            return (VenueShowCell)Nib.Instantiate(null, null)[0];
        }
    }
}