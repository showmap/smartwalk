using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Core.Model;
using SmartWalk.iOS.Views.Common;
using SmartWalk.iOS.Resources;

namespace SmartWalk.iOS.Views.OrgView
{
    public partial class OrgEventCell : TableCellBase
    {
        public static readonly UINib Nib = UINib.FromName("OrgEventCell", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("OrgEventCell");

        public const float DefaultHeight = 44;

        public OrgEventCell(IntPtr handle) : base(handle)
        {
            SelectedBackgroundView = new UIView { BackgroundColor = Theme.CellHighlight };
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
            InitializeLabels();
        }

        protected override void OnDataContextChanged(object previousContext, object newContext)
        {
            WeekDayLabel.Text = DataContext != null ? string.Format("{0:ddd}", DataContext.Date).ToUpper() : null; // TODO: probably only for EN locale
            DayLabel.Text = DataContext != null ? DataContext.Date.Day.ToString() : null;
            DateLabel.Text = DataContext != null ? string.Format("{0:d MMMM yyyy}", DataContext.Date) : null;
            HintLabel.Text = DataContext != null && DataContext.HasSchedule ? null : "no schedule";

            CalendarView.BackgroundColor = DataContext != null && !DataContext.HasSchedule 
                ? Theme.OrgEventPassive : Theme.OrgEventActive;
        }

        private void InitializeLabels()
        {
            WeekDayLabel.Font = Theme.OrgEventWeekDayFont;
            WeekDayLabel.TextColor = Theme.OrgEventDayText;

            DayLabel.Font = Theme.OrgEventDayFont;
            DayLabel.TextColor = Theme.OrgEventDayText;

            DateLabel.Font = Theme.OrgEventDateFont;
            DateLabel.TextColor = Theme.OrgEventDateText;

            HintLabel.Font = Theme.OrgEventHintFont;
            HintLabel.TextColor = Theme.OrgEventHintText;
        }
    }
}