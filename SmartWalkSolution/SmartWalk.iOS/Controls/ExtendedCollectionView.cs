using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.iOS.Utils;
using SmartWalk.Core.Utils;

namespace SmartWalk.iOS.Controls
{
    [Register("ExtendedCollectionView")]
    public class ExtendedCollectionView : UICollectionView
    {
        public ExtendedCollectionView(IntPtr handle) : base(handle)
        {
        }

        public ExtendedCollectionView(RectangleF frame, UICollectionViewLayout layout) : base(frame, layout)
        {
        }

        public float CellHeight { get; set; }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            SetCellWidth();
        }

        private void SetCellWidth()
        {
            var flowLayout = (UICollectionViewFlowLayout)CollectionViewLayout;
            var itemsInRow = ScreenUtil.IsVerticalOrientation ? 1 : 2;

            var cellWith = (Frame.Width - 
                            flowLayout.SectionInset.Left -
                            flowLayout.SectionInset.Right - 
                            flowLayout.MinimumInteritemSpacing * (itemsInRow - 1)) / itemsInRow;

            flowLayout.ItemSize = new SizeF(cellWith, CellHeight);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            ConsoleUtil.LogDisposed(this);
        }
    }
}