using System;
using Foundation;
using UIKit;
using SmartWalk.Client.iOS.Resources;
using SmartWalk.Client.iOS.Utils;
using SmartWalk.Client.iOS.Views.Common.Base.Cells;

namespace SmartWalk.Client.iOS.Views.Common.GroupHeader
{
    public class GroupHeaderView : TableHeaderBase
    {
        public static readonly NSString Key = new NSString("GroupHeaderContentView");

        public const float DefaultHeight = 28;

        public GroupHeaderView(IntPtr handle) : base(handle)
        {
            BackgroundView = new UIView { BackgroundColor = ThemeColors.ContentLightBackgroundAlpha };
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