using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Core.Model;
using SmartWalk.iOS.Views.Common;

namespace SmartWalk.iOS.Views.OrgView
{
    public partial class OrgEventCell : TableCellBase
    {
        public static readonly UINib Nib = UINib.FromName("OrgEventCell", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("OrgEventCell");

        public OrgEventCell(IntPtr handle) : base(handle)
        {
        }

        public new OrgEventInfo DataContext
        {
            get { return (OrgEventInfo)base.DataContext; }
            set { base.DataContext = value; }
        }

        public static OrgEventCell Create()
        {
            return (OrgEventCell)Nib.Instantiate(null, null)[0];
        }

        protected override void OnInitialize()
        {
            InitializeDayLabelSquare();
        }

        protected override void OnDataContextChanged()
        {
            WeekDayLabel.Text = DataContext != null ? string.Format("{0:ddd}", DataContext.Date) : null;
            DayLabel.Text = DataContext != null ? string.Format("{0:dd}", DataContext.Date) : null;
            DateLabel.Text = DataContext != null ? string.Format("{0:d MMMM yyyy}", DataContext.Date) : null;
            HintLabel.Text = DataContext != null && DataContext.HasSchedule ? null : "(no schedule)";

            DateLabel.TextColor = DataContext != null && !DataContext.HasSchedule 
                ? UIColor.LightGray : UIColor.Black;
            DayLabel.BackgroundColor = DataContext != null && !DataContext.HasSchedule 
                ? UIColor.FromRGB(128, 128, 128) : UIColor.FromRGB(0, 128, 255);
        }

        private void InitializeDayLabelSquare()
        {
            DayLabel.Layer.CornerRadius = 3;
        }
    }
}