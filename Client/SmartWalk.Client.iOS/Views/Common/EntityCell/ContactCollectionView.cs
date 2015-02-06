using System;
using CoreGraphics;
using Foundation;
using UIKit;
using SmartWalk.Client.iOS.Utils;

namespace SmartWalk.Client.iOS.Views.Common.EntityCell
{
    [Register("ContactCollectionView")]
    public class ContactCollectionView : UICollectionView
    {
        public ContactCollectionView(IntPtr handle) : base(handle)
        {
        }

        public ContactCollectionView(CGRect frame, UICollectionViewLayout layout) : base(frame, layout)
        {
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            if (Frame.Width != 0 && Frame.Height != 0)
            {
                UpdateLayoutSizes();
            }
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

            var size = new CGSize(cellWidth, ContactCell.DefaultHeight);
            if (flowLayout.ItemSize != size) {
                flowLayout.ItemSize = size;
                flowLayout.InvalidateLayout();
            }
        }
    }
}