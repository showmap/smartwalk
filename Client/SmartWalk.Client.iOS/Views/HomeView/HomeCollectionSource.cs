using Cirrious.MvvmCross.Binding.Touch.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Core.Model;
using SmartWalk.iOS.Controls;

namespace SmartWalk.iOS.Views.HomeView
{
    public class HomeCollectionSource : MvxCollectionViewSource, IListViewSource
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