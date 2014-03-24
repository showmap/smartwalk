using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using SmartWalk.Client.iOS.Resources;

namespace SmartWalk.Client.iOS.Views.Common
{
    public partial class GroupHeaderView : TableHeaderBase
    {
        public static readonly UINib Nib = UINib.FromName("GroupHeaderView", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("GroupHeaderView");

        public const float DefaultHeight = 26;

        public GroupHeaderView(IntPtr handle) : base(handle)
        {
            BackgroundView = new UIView { BackgroundColor = Theme.HeaderCellBackground };
        }

        public static GroupHeaderView Create()
        {
            return (GroupHeaderView)Nib.Instantiate(null, null)[0];
        }

        protected override void OnDataContextChanged(object previousContext, object newContext)
        {
            var text = newContext as string;
            TitleLabel.Text = text != null ? text.ToUpper() : null;
        }

        protected override void OnInitialize()
        {
            BottomSeparator.IsLineOnTop = true;
        }
    }
}