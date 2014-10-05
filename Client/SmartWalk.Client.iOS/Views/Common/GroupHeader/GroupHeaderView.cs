using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Client.iOS.Resources;
using SmartWalk.Client.iOS.Utils;
using SmartWalk.Client.iOS.Views.Common.Base.Cells;

namespace SmartWalk.Client.iOS.Views.Common.GroupHeader
{
    public class GroupHeaderView : TableHeaderBase
    {
        public static readonly NSString Key = new NSString("GroupHeaderContentView");

        public const float DefaultHeight = 26;

        public GroupHeaderView(IntPtr handle) : base(handle)
        {
            BackgroundView = new UIView { BackgroundColor = Theme.HeaderCellBackground };
            ContentView = GroupHeaderContentView.Create();
            Frame = ContentView.Bounds;
            base.ContentView.RemoveSubviews();
            base.ContentView.Add(ContentView);
        }

        protected new GroupHeaderContentView ContentView { get; private set; }

        protected override void OnDataContextChanged(object previousContext, object newContext)
        {
            ContentView.DataContext = newContext;
        }
    }
}