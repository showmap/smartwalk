using System.Windows.Input;
using Cirrious.MvvmCross.Binding.Touch.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Core.Model;

namespace SmartWalk.iOS.Views.Common.EntityCell
{
    public class EntityCollectionSource : MvxCollectionViewSource
    {
        public EntityCollectionSource(UICollectionView collectionView) : 
            base(collectionView)
        {
            collectionView.RegisterNibForCell(ImageCell.Nib, ImageCell.Key);
            collectionView.RegisterNibForCell(MapCell.Nib, MapCell.Key);
        }

        public ICommand ShowImageFullscreenCommand { get; set; }
        public ICommand NavigateWebSiteCommand { get; set; }
        public ICommand NavigateAddressesCommand { get; set; }

        protected override UICollectionViewCell GetOrCreateCellFor(
            UICollectionView collectionView, 
            NSIndexPath indexPath, 
            object item)
        {
            var cell = default(UICollectionViewCell);

            var imageItem = item as ImageCollectionItem;
            if (imageItem != null)
            {
                cell = (UICollectionViewCell)collectionView.DequeueReusableCell(ImageCell.Key, indexPath);
                ((ImageCell)cell).DataContext = imageItem.EntityInfo;
                ((ImageCell)cell).ShowImageFullscreenCommand = ShowImageFullscreenCommand;
                ((ImageCell)cell).NavigateWebSiteCommand = NavigateWebSiteCommand;
            }

            var mapItem = item as MapCollectionItem;
            if (mapItem != null)
            {
                cell = (UICollectionViewCell)collectionView.DequeueReusableCell(MapCell.Key, indexPath);
                ((MapCell)cell).DataContext = mapItem.Entity;
                ((MapCell)cell).NavigateAddressesCommand = NavigateAddressesCommand;
            }

            return cell;
        }
    }

    public class ImageCollectionItem
    {
        public EntityInfo EntityInfo { get; set; }
    }

    public class MapCollectionItem
    {
        public Entity Entity { get; set; }
    }
}