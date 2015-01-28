using System;
using UIKit;
using Foundation;
using SmartWalk.Client.iOS.Views.Common.Base.Cells;
using SmartWalk.Client.iOS.Resources;

namespace SmartWalk.Client.iOS.Views.Common.GroupHeader
{
    public partial class GroupHeaderContentView : ContentViewBase
    {
        public static readonly UINib Nib = UINib.FromName("GroupHeaderContentView", NSBundle.MainBundle);

        public GroupHeaderContentView(IntPtr handle) : base(handle)
        {
        }

        public static GroupHeaderContentView Create()
        {
            return (GroupHeaderContentView)Nib.Instantiate(null, null)[0];
        }

        protected override void OnDataContextChanged(object previousContext, object newContext)
        {
            var text = newContext as string;
            TitleLabel.Text = text != null ? text.ToUpper() : null;
        }

        protected override void OnInitialize()
        {
            TitleLabel.Font = Theme.GroupHeaderTextFont;
            TitleLabel.TextColor = ThemeColors.Metadata;
        }
    }
}