using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.iOS.Utils;
using SmartWalk.Core.Utils;

namespace SmartWalk.iOS.Views.HomeView
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

        public float? CellHeight { get; set; }

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

            flowLayout.ItemSize = new SizeF(cellWith, CellHeight ?? 100);
        }
    }
}