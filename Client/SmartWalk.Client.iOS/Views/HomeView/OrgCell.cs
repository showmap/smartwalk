using System;
using Foundation;
using UIKit;
using SmartWalk.Client.Core.Model;
using SmartWalk.Client.Core.Utils;
using SmartWalk.Client.iOS.Views.Common.Base.Cells;
using SmartWalk.Client.iOS.Resources;

namespace SmartWalk.Client.iOS.Views.HomeView
{
    public partial class OrgCell : CollectionCellBase
    {
        public static readonly UINib Nib = UINib.FromName("OrgCell", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("OrgCell");

        public const float DefaultWidth = 320f;
        public const float DefaultHeight = 150f;

        public OrgCell(IntPtr handle) : base(handle)
        {
        }

        public new OrgEvent DataContext
        {
            get { return (OrgEvent)base.DataContext; }
            set { base.DataContext = value; }
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            ImageBackground.ResizeImage = true;
            InitializeStyle();
        }

        public override void WillMoveToSuperview(UIView newsuper)
        {
            base.WillMoveToSuperview(newsuper);

            if (newsuper == null)
            {
                DisposeImageBackground();
            }
        }

        protected override void OnDataContextChanged()
        {
            ImageBackground.ImageUrl = DataContext != null ? DataContext.Picture : null;
            ImageBackground.Title = DataContext != null ? DataContext.Title : null;
            ImageBackground.Subtitle = DataContext != null ? DataContext.GetDateString() : null;
        }

        private void DisposeImageBackground()
        {
            ImageBackground.Dispose();
        }

        private void InitializeStyle()
        {
            ImageBackground.BackgroundColor = ThemeColors.ContentLightHighlight;
        }
    }
}