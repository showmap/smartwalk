using Cirrious.MvvmCross.Binding.Touch.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Core.Model.Interfaces;

namespace SmartWalk.iOS.Views.Common.EntityCell
{
    public class ContactCollectionSource : MvxCollectionViewSource
    {
        public ContactCollectionSource(UICollectionView collectionView) : 
            base(collectionView)
        {
            collectionView.RegisterNibForCell(ContactCell.Nib, ContactCell.Key);
        }

        protected override UICollectionViewCell GetOrCreateCellFor(
            UICollectionView collectionView, 
            NSIndexPath indexPath, 
            object item)
        {
            var cell = default(UICollectionViewCell);

            var contact = item as IContact;
            if (contact != null)
            {
                cell = (UICollectionViewCell)collectionView.DequeueReusableCell(ContactCell.Key, indexPath);
                ((ContactCell)cell).DataContext = contact;
            }

            return cell;
        }
    }
}