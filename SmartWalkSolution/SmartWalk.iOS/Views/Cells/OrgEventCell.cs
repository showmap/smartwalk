using System;
using Cirrious.MvvmCross.Binding.Touch.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace SmartWalk.iOS.Views.Cells
{
    public partial class OrgEventCell : MvxTableViewCell
    {
        public static readonly UINib Nib = UINib.FromName("OrgEventCell",
            NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("OrgEventCell");

        public const string BindingText = @"
Day Day;
Month Month";

        public OrgEventCell() : base(BindingText)
        {    
        }

        public OrgEventCell(IntPtr handle) : base(BindingText, handle)
        {
        }

        public static OrgEventCell Create()
        {
            return (OrgEventCell)Nib.Instantiate(null, null)[0];
        }

        public string Day {
            get { return DayLabel.Text; }
            set { DayLabel.Text = value; }
        }

        public string Month {
            get { return MonthLabel.Text; }
            set { MonthLabel.Text = value; }
        }
    }
}