using System;
using Cirrious.MvvmCross.Binding.BindingContext;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Core.Model;
using SmartWalk.Core.Converters;
using SmartWalk.Core.Utils;
using SmartWalk.iOS.Views.Common;

namespace SmartWalk.iOS.Views.OrgView
{
    public partial class OrgEventCell : TableCellBase
    {
        public static readonly UINib Nib = UINib.FromName("OrgEventCell", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("OrgEventCell");

        public OrgEventCell(IntPtr handle) : base(handle)
        {
            this.DelayBind(() => {
                var set = this.CreateBindingSet<OrgEventCell, OrgEventInfo>();
                set.Bind(WeekDayLabel).To(oei => oei.Date).WithConversion(new DateTimeFormatConverter(), "ddd");
                set.Bind(DayLabel).To(oei => oei.Date).WithConversion(new DateTimeFormatConverter(), "dd");
                set.Bind(DateLabel).To(oei => oei.Date).WithConversion(new DateTimeFormatConverter(), "d MMMM yyyy");
                set.Bind(HintLabel).WithConversion(new ValueConverter<OrgEventInfo>(
                    ei => ei != null && ei.HasSchedule ? null : "(no schedule)"), null);
                set.Apply();
            });
        }
        public static OrgEventCell Create()
        {
            return (OrgEventCell)Nib.Instantiate(null, null)[0];
        }
    }
}