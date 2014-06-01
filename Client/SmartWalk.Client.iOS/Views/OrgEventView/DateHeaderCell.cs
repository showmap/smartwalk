using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Client.iOS.Resources;
using SmartWalk.Client.iOS.Views.Common.Base;
using SmartWalk.Client.iOS.Views.Common.GroupHeader;

namespace SmartWalk.Client.iOS.Views.OrgEventView
{
    public class DateHeaderCell : TableCellBase
    {
        public static readonly NSString Key = new NSString("DateHeaderCell");

        public const float DefaultHeight = 26;

        public DateHeaderCell(IntPtr handle) : base(handle)
        {
            BackgroundView = new UIView { BackgroundColor = Theme.HeaderCellBackground };
            ContentView = GroupHeaderContentView.Create();
            Frame = ContentView.Bounds;
            base.ContentView.Add(ContentView);
        }

        protected new GroupHeaderContentView ContentView { get; private set; }

        protected override void OnDataContextChanged(object previousContext, object newContext)
        {
            ContentView.DataContext = newContext;
        }
    }
}