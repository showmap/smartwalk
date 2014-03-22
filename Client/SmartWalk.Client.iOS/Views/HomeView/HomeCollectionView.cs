using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Client.iOS.Utils;
using SmartWalk.Client.Core.Utils;

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
            SetCellWidth();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            ConsoleUtil.LogDisposed(this);
        }

        private void SetCellWidth()
        {
            var flowLayout = (UICollectionViewFlowLayout)CollectionViewLayout;
            var itemsInRow = ScreenUtil.IsVerticalOrientation ? 1 : 2;

            var cellWith = (Frame.Width - 
                            flowLayout.SectionInset.Left -
                            flowLayout.SectionInset.Right - 
                            flowLayout.MinimumInteritemSpacing * (itemsInRow - 1)) / itemsInRow;

            flowLayout.ItemSize = new SizeF(cellWith, OrgCell.DefaultHeight);
        }
    }
}