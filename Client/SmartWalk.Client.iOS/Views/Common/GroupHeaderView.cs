using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Client.iOS.Resources;

namespace SmartWalk.Client.iOS.Views.Common
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
            base.ContentView.Add(ContentView);
        }

        protected new GroupHeaderContentView ContentView { get; private set; }

        protected override void OnDataContextChanged(object previousContext, object newContext)
        {
            ContentView.DataContext = newContext;
        }
    }
}