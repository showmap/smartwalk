using Cirrious.MvvmCross.Binding.Touch.Views;
using Foundation;
using UIKit;
using SmartWalk.Client.Core.Model.DataContracts;

namespace SmartWalk.Client.iOS.Views.Common.EntityCell
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

            var contact = item as Contact;
            if (contact != null)
            {
                cell = (UICollectionViewCell)collectionView.DequeueReusableCell(ContactCell.Key, indexPath);
                ((ContactCell)cell).DataContext = contact;
            }

            return cell;
        }
    }
}