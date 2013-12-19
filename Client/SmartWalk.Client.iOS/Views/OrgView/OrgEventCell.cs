using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Client.Core.Model;
using SmartWalk.Client.iOS.Views.Common;
using SmartWalk.Client.iOS.Resources;

namespace SmartWalk.Client.iOS.Views.OrgView
{
    public partial class OrgEventCell : TableCellBase
    {
        public static readonly UINib Nib = UINib.FromName("OrgEventCell", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("OrgEventCell");

        public const float DefaultHeight = 44;

        public OrgEventCell(IntPtr handle) : base(handle)
        {   
            BackgroundView = new UIView { BackgroundColor = Theme.BackgroundPatternColor };
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
            InitializeLabelsStyle();
        }

        protected override void OnDataContextChanged(object previousContext, object newContext)
        {
            WeekDayLabel.Text = DataContext != null 
                ? string.Format("{0:ddd}", DataContext.Date).ToUpper() 
                : null;

            DayLabel.Text = DataContext != null 
                ? DataContext.Date.Day.ToString() 
                : null;

            DateLabel.Text = DataContext != null 
                ? string.Format("{0:d MMMM yyyy}", DataContext.Date) 
                : null;

            DateLabel.TextColor = DataContext != null && DataContext.HasSchedule 
                ? Theme.CellText
                : Theme.CellTextPassive;

            HintLabel.Text = DataContext != null && DataContext.HasSchedule 
                ? null 
                : "no schedule";

            CalendarView.BackgroundColor = DataContext != null && !DataContext.HasSchedule 
                ? Theme.OrgEventPassive : Theme.OrgEventActive;

            UserInteractionEnabled = DataContext != null && DataContext.HasSchedule;
        }

        private void InitializeLabelsStyle()
        {
            WeekDayLabel.Font = Theme.OrgEventWeekDayFont;
            WeekDayLabel.TextColor = Theme.CellTextHighlight;

            DayLabel.Font = Theme.OrgEventDayFont;
            DayLabel.TextColor = Theme.CellTextHighlight;

            DateLabel.Font = Theme.OrgEventDateFont;
            DateLabel.TextColor = Theme.CellText;

            HintLabel.Font = Theme.OrgEventHintFont;
            HintLabel.TextColor = Theme.CellTextHint;
        }
    }
}