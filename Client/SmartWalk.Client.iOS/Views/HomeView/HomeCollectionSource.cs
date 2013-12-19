using Cirrious.MvvmCross.Binding.Touch.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Client.Core.Model;
using SmartWalk.Client.iOS.Controls;

namespace SmartWalk.Client.iOS.Views.HomeView
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