using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Client.Core.Model;
using SmartWalk.Client.Core.Utils;
using SmartWalk.Client.iOS.Views.Common;
using SmartWalk.Client.iOS.Views.Common.Base.Cells;

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

        private ImageBackgroundView ImageBackground
        {
            get { return (ImageBackgroundView)Placeholder.Content; }
        }

        public static OrgCell Create()
        {
            return (OrgCell)Nib.Instantiate(null, null)[0];
        }

        public override void WillMoveToSuperview(UIView newsuper)
        {
            base.WillMoveToSuperview(newsuper);

            if (newsuper == null)
            {
                DisposeHeaderImage();
            }
        }

        protected override void OnInitialize()
        {
            InitializeHeaderImage();
        }

        protected override void OnDataContextChanged()
        {
            ImageBackground.ImageUrl = DataContext != null ? DataContext.Picture : null;
            ImageBackground.Title = DataContext != null ? DataContext.Title : null;
            ImageBackground.Subtitle = DataContext != null ? DataContext.GetDateString() : null;
        }

        private void InitializeHeaderImage()
        {
            var view = ImageBackgroundView.Create();
            // Making sure that it has proper frame for loading a resized image
            view.Frame = Bounds;

            Placeholder.Content = view;
            ImageBackground.Initialize(true);
        }

        private void DisposeHeaderImage()
        {
            ImageBackground.Dispose();
            Placeholder.Content = null;
        }
    }
}