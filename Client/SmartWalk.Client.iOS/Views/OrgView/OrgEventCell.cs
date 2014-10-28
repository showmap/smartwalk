using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Client.Core.Model;
using SmartWalk.Client.iOS.Resources;
using SmartWalk.Client.iOS.Views.Common.Base.Cells;

namespace SmartWalk.Client.iOS.Views.OrgView
{
    public partial class OrgEventCell : TableCellBase
    {
        public static readonly UINib Nib = UINib.FromName("OrgEventCell", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("OrgEventCell");

        public const float DefaultHeight = 44;

        public OrgEventCell(IntPtr handle) : base(handle)
        {   
            BackgroundView = new UIView { BackgroundColor = UIColor.White };
            SelectedBackgroundView = new UIView { BackgroundColor = Theme.CellHighlight };
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
            InitializeLabelsStyle();
        }

        protected override void OnDataContextChanged(object previousContext, object newContext)
        {
            MonthLabel.Text = DataContext != null && DataContext.StartTime.HasValue
                ? string.Format("{0:MMM}", DataContext.StartTime.Value).ToUpper() 
                : null;

            DayLabel.Text = DataContext != null && DataContext.StartTime.HasValue
                ? DataContext.StartTime.Value.Day.ToString() 
                : null;

            EventTitleLabel.Text = DataContext != null
                ? string.Format("{0}", DataContext.Title) 
                : null;
        }

        private void InitializeLabelsStyle()
        {
            MonthLabel.Font = Theme.OrgEventMonthFont;
            MonthLabel.TextColor = Theme.CellTextHighlight;

            DayLabel.Font = Theme.OrgEventDayFont;
            DayLabel.TextColor = Theme.CellTextHighlight;

            EventTitleLabel.Font = Theme.OrgEventTitleFont;
            EventTitleLabel.TextColor = Theme.CellText;
        }
    }
}