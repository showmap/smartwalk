using Cirrious.MvvmCross.Binding.Touch.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Core.Model;
using System.Windows.Input;

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

        public ICommand NavigateSiteLinkCommand { get; set; }

        protected override UICollectionViewCell GetOrCreateCellFor(
            UICollectionView collectionView, 
            NSIndexPath indexPath, 
            object item)
        {
            var cell = default(UICollectionViewCell);

            var phoneInfo = item as PhoneInfo;
            if (phoneInfo != null)
            {
                cell = (UICollectionViewCell)collectionView.DequeueReusableCell(PhoneCell.Key, indexPath);
                ((PhoneCell)cell).DataContext = phoneInfo;
            }

            var emailInfo = item as EmailInfo;
            if (emailInfo != null)
            {
                cell = (UICollectionViewCell)collectionView.DequeueReusableCell(EmailCell.Key, indexPath);
                ((EmailCell)cell).DataContext = emailInfo;
            }

            var webSiteInfo = item as WebSiteInfo;
            if (webSiteInfo != null)
            {
                cell = (UICollectionViewCell)collectionView.DequeueReusableCell(WebSiteCell.Key, indexPath);
                ((WebSiteCell)cell).DataContext = webSiteInfo;
                ((WebSiteCell)cell).NavigateSiteLinkCommand = NavigateSiteLinkCommand;
            }

            return cell;
        }
    }
}