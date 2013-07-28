using Cirrious.MvvmCross.Binding.Touch.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Core.Model;

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
            var cell = (OrgCell)collectionView.DequeueReusableCell(OrgCell.Key, indexPath);
            cell.DataContext = (EntityInfo)item;
            return cell;
        }
    }
}