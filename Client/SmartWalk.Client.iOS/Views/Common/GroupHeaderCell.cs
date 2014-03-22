using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Client.iOS.Resources;

namespace SmartWalk.Client.iOS.Views.Common
{
    public partial class GroupHeaderCell : TableHeaderBase
    {
        public static readonly UINib Nib = UINib.FromName("GroupHeaderCell", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("GroupHeaderCell");

        public const float DefaultHeight = 26;

        public GroupHeaderCell(IntPtr handle) : base(handle)
        {
            BackgroundView = new UIView { BackgroundColor = Theme.HeaderCellBackground };
        }

        public static GroupHeaderCell Create()
        {
            return (GroupHeaderCell)Nib.Instantiate(null, null)[0];
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