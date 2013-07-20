using System;
using Cirrious.MvvmCross.Binding;
using Cirrious.MvvmCross.Binding.Binders;
using Cirrious.MvvmCross.Binding.Touch.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Core.Model;
using SmartWalk.Core.Utils;
using SmartWalk.Core.Converters;

namespace SmartWalk.iOS.Views.OrgView
{
    public partial class OrgEventCell : MvxTableViewCell
    {
        public static readonly UINib Nib = UINib.FromName("OrgEventCell", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("OrgEventCell");

        private static readonly MvxBindingDescription[] Bindings = new [] {
            new MvxBindingDescription(
                Reflect<OrgEventCell>.GetProperty(p => p.WeekDayText).Name,
                Reflect<OrgEventInfo>.GetProperty(p => p.Date).Name,
                new DateTimeFormatConverter(), "ddd", null, MvxBindingMode.OneWay),
            new MvxBindingDescription(
                Reflect<OrgEventCell>.GetProperty(p => p.DayText).Name,
                Reflect<OrgEventInfo>.GetProperty(p => p.Date).Name,
                new DateTimeFormatConverter(), "dd", null, MvxBindingMode.OneWay),
            new MvxBindingDescription(
                Reflect<OrgEventCell>.GetProperty(p => p.DateText).Name,
                Reflect<OrgEventInfo>.GetProperty(p => p.Date).Name,
                new DateTimeFormatConverter(), "d MMMM yyyy", null, MvxBindingMode.OneWay),
            new MvxBindingDescription(
                Reflect<OrgEventCell>.GetProperty(p => p.HintText).Name,
                null,
                new OrgEventHintConverter(), null, null, MvxBindingMode.OneWay)
        };

        public OrgEventCell() : base(Bindings)
        {    
        }

        public OrgEventCell(IntPtr handle) : base (Bindings, handle)
        {
        }

        public string WeekDayText {
            get { return WeekDayLabel.Text; }
            set { WeekDayLabel.Text = value; }
        }

        public string DayText {
            get { return DayLabel.Text; }
            set { DayLabel.Text = value; }
        }

        public string DateText {
            get { return DateLabel.Text; }
            set { DateLabel.Text = value; }
        }

        public string HintText {
            get { return HintLabel.Text; }
            set { HintLabel.Text = value; }
        }

        public static OrgEventCell Create()
        {
            return (OrgEventCell)Nib.Instantiate(null, null)[0];
        }
    }
}