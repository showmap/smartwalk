using System;
using Foundation;
using UIKit;
using SmartWalk.Client.Core.Model;
using SmartWalk.Client.iOS.Resources;
using SmartWalk.Client.iOS.Views.Common.Base.Cells;
using SmartWalk.Shared.Utils;

namespace SmartWalk.Client.iOS.Views.OrgView
{
    public partial class OrgEventCell : TableCellBase
    {
        public static readonly UINib Nib = UINib.FromName("OrgEventCell", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("OrgEventCell");

        public const float DefaultHeight = 50;

        public OrgEventCell(IntPtr handle) : base(handle)
        {   
            BackgroundView = new UIView { BackgroundColor = UIColor.White };
            SelectedBackgroundView = new UIView { BackgroundColor = ThemeColors.ContentLightHighlight };
        }

        public static OrgEventCell Create()
        {
            return (OrgEventCell)Nib.Instantiate(null, null)[0];
        }

        public new OrgEvent DataContext
        {
            get { return (OrgEvent)base.DataContext; }
            set { base.DataContext = value; }
        }

        public bool IsSeparatorVisible
        {
            get { return !Separator.Hidden; }
            set { Separator.Hidden = !value; }
        }

        protected override void OnInitialize()
        {
            InitializeStyle();
        }

        protected override void OnDataContextChanged(object previousContext, object newContext)
        {
            DateLabel.AttributedText = DataContext.GetOrgEventDateString(
                ThemeColors.ContentLightTextPassive);

            EventTitleLabel.Text = DataContext != null
                ? string.Format("{0}", DataContext.Title) 
                : null;
        }

        private void InitializeStyle()
        {
            EventTitleLabel.Font = Theme.ContentFont;
            EventTitleLabel.TextColor = ThemeColors.ContentLightText;

            CalendarView.LineColor = ThemeColors.BorderDark;
            CalendarView.LineWidth = 1;
        }
    }

    public static class OrgEventCellExtensions
    {
        public static NSAttributedString GetOrgEventDateString(
            this OrgEvent orgEvent, 
            UIColor textColor,
            int? currentDay = null,
            bool isVertical = true)
        {
            if (orgEvent == null || !orgEvent.StartTime.HasValue) return new NSAttributedString();

            var daysCount = DateTimeExtensions.DaysCount(orgEvent.StartTime, orgEvent.EndTime);
            var result = new NSMutableAttributedString();

            result.Append(
                new NSAttributedString(
                    string.Format("{0:MMM}{1}", 
                        orgEvent.StartTime.Value,
                        Environment.NewLine).ToUpper(),
                    isVertical
                        ? Theme.OrgEventMonthFont
                        : Theme.OrgEventMonthLandscapeFont,
                    textColor,
                    null,
                    null,
                    new NSMutableParagraphStyle { 
                        Alignment = UITextAlignment.Center
                    }));

            if (daysCount > 1 && currentDay == null)
            {
                result.Append(
                    new NSAttributedString(
                        string.Format("{0}-{1}", 
                            orgEvent.StartTime.Value.Day,
                            orgEvent.EndTime.Value.Day),
                        isVertical
                            ? Theme.OrgEventTwoDaysFont
                            : Theme.OrgEventTwoDaysLandscapeFont,
                        textColor,
                        null,
                        null,
                        new NSMutableParagraphStyle { 
                            Alignment = UITextAlignment.Center,
                            LineHeightMultiple = 0.85f
                        }));
            }
            else
            {
                result.Append(
                    new NSAttributedString(
                        string.Format("{0}", 
                            orgEvent.StartTime.Value.AddDays((currentDay ?? 1) - 1).Day),
                        isVertical
                            ? Theme.OrgEventDayFont
                            : Theme.OrgEventDayLandscapeFont,
                        textColor,
                        null,
                        null,
                        new NSMutableParagraphStyle { 
                            Alignment = UITextAlignment.Center,
                            LineHeightMultiple = 0.9f
                        }));
            }

            return result;
        }
    }
}