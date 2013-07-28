using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace SmartWalk.iOS.Views.Common.EntityCell
{
    public class ContactCollectionDelegate : UICollectionViewDelegate
    {
        public override void ItemHighlighted(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = collectionView.CellForItem(indexPath);
            cell.ContentView.BackgroundColor = UIColor.FromRGB(245, 245, 245);
        }

        public override void ItemUnhighlighted(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = collectionView.CellForItem(indexPath);
            cell.ContentView.BackgroundColor = null;
        }
    }
}