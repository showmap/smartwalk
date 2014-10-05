using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Client.Core.Model.DataContracts;
using SmartWalk.Client.Core.Utils;
using SmartWalk.Client.iOS.Resources;
using SmartWalk.Client.iOS.Utils;
using SmartWalk.Client.iOS.Views.Common.Base.Cells;
using SmartWalk.Client.iOS.Views.Common.GroupHeader;

namespace SmartWalk.Client.iOS.Views.OrgEventView
{
    public class DayHeaderCell : TableCellBase
    {
        public static readonly NSString Key = new NSString("DayHeaderCell");

        public const float DefaultHeight = 22;

        public DayHeaderCell(IntPtr handle) : base(handle)
        {
            BackgroundView = new UIView { BackgroundColor = Theme.HeaderCellBackground };
            ContentView = GroupHeaderContentView.Create();
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