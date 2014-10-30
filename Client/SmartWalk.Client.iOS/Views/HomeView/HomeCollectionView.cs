using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Client.iOS.Utils;

namespace SmartWalk.Client.iOS.Views.HomeView
{
    [Register("HomeCollectionView")]
    public class HomeCollectionView : UICollectionView
    {
        public HomeCollectionView(IntPtr handle) : base(handle)
        {
        }

        public HomeCollectionView(RectangleF frame, UICollectionViewLayout layout) : base(frame, layout)
        {
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            UpdateLayoutSizes();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            ConsoleUtil.LogDisposed(this);
        }

        private void UpdateLayoutSizes()
        {
            var flowLayout = (UICollectionViewFlowLayout)CollectionViewLayout;
            var itemsInRow = ScreenUtil.IsVerticalOrientation ? 1 : 2;

            var cellWidth = (Frame.Width - 
                            flowLayout.SectionInset.Left -
                            flowLayout.SectionInset.Right - 
                            flowLayout.MinimumInteritemSpacing * (itemsInRow - 1)) / itemsInRow;

            var cellHeight = ScreenUtil.GetProportionalHeight(
                new SizeF(OrgCell.DefaultWidth, OrgCell.DefaultHeight), Frame.Size);

            flowLayout.ItemSize = new SizeF(cellWidth, cellHeight);
            flowLayout.HeaderReferenceSize = new SizeF(Frame.Width, HomeHeaderView.DefaultHeight);
        }
    }
}