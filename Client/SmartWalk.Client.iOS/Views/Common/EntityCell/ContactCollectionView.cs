using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Client.iOS.Utils;

namespace SmartWalk.Client.iOS.Views.Common.EntityCell
{
    [Register("ContactCollectionView")]
    public class ContactCollectionView : UICollectionView
    {
        public ContactCollectionView(IntPtr handle) : base(handle)
        {
        }

        public ContactCollectionView(RectangleF frame, UICollectionViewLayout layout) : base(frame, layout)
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
            var cellWidth = Frame.Width - flowLayout.SectionInset.Left - flowLayout.SectionInset.Right;

            var size = new SizeF(cellWidth, ContactCell.DefaultHeight);
            if (flowLayout.ItemSize != size) {
                flowLayout.ItemSize = size;
                flowLayout.InvalidateLayout();
            }
        }
    }
}