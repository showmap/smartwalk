using Cirrious.MvvmCross.Binding.Touch.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Core.Model;

namespace SmartWalk.iOS.Views.Common.EntityCell
{
    public class ContactCollectionSource : MvxCollectionViewSource
    {
        public ContactCollectionSource(UICollectionView collectionView) : 
            base(collectionView)
        {
            collectionView.RegisterNibForCell(PhoneCell.Nib, PhoneCell.Key);
            collectionView.RegisterNibForCell(EmailCell.Nib, EmailCell.Key);
            collectionView.RegisterNibForCell(WebSiteCell.Nib, WebSiteCell.Key);
        }

        protected override UICollectionViewCell GetOrCreateCellFor(UICollectionView collectionView, NSIndexPath indexPath, object item)
        {
            if (item is PhoneInfo)
            {
                return (UICollectionViewCell)collectionView.DequeueReusableCell(PhoneCell.Key, indexPath);
            }

            if (item is EmailInfo)
            {
                return (UICollectionViewCell)collectionView.DequeueReusableCell(EmailCell.Key, indexPath);
            }

            if (item is WebSiteInfo)
            {
                return (UICollectionViewCell)collectionView.DequeueReusableCell(WebSiteCell.Key, indexPath);
            }

            return null;
        }
    }
}