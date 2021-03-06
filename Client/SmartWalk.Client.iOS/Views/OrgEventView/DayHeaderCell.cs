using System;
using Foundation;
using SmartWalk.Client.Core.Model.DataContracts;
using SmartWalk.Client.Core.Utils;
using SmartWalk.Client.iOS.Resources;
using SmartWalk.Client.iOS.Utils;
using SmartWalk.Client.iOS.Views.Common.Base.Cells;
using SmartWalk.Client.iOS.Views.Common.GroupHeader;
using UIKit;

namespace SmartWalk.Client.iOS.Views.OrgEventView
{
    public class DayHeaderCell : TableCellBase
    {
        public static readonly NSString Key = new NSString("DayHeaderCell");

        public const float DefaultHeight = 28;

        public DayHeaderCell(IntPtr handle) : base(handle)
        {
            SelectionStyle = UITableViewCellSelectionStyle.None;
            ContentView = GroupHeaderContentView.Create();
            ContentView.SeparatorBackgroundColor = ThemeColors.ContentLightBackground;
            Frame = ContentView.Bounds;
            base.ContentView.RemoveSubviews();
            base.ContentView.Add(ContentView);
        }

        public new Show DataContext
        {
            get { return (Show)base.DataContext; }
            set { base.DataContext = value; }
        }

        protected new GroupHeaderContentView ContentView { get; private set; }

        protected override void OnDataContextChanged(object previousContext, object newContext)
        {
            ContentView.DataContext = 
                DataContext != null 
                    ? DataContext.StartTime.Value.GetCurrentDayString()
                    : null;
        }
    }
}