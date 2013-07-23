using Cirrious.MvvmCross.Binding.Touch.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace SmartWalk.iOS.Views.HomeView
{
    public class HomeCollectionSource : MvxCollectionViewSource
    {
        public HomeCollectionSource(UICollectionView collectionView)
            : base(collectionView)
        {
            collectionView.RegisterNibForCell(OrgCell.Nib, OrgCell.Key);
        }

        protected override UICollectionViewCell GetOrCreateCellFor(
            UICollectionView collectionView, 
            NSIndexPath indexPath, 
            object item)
        {
            return (UICollectionViewCell)collectionView.DequeueReusableCell(OrgCell.Key, indexPath);
        }
    }
}