using System.Linq;
using System.Windows.Input;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Core.Model;

namespace SmartWalk.iOS.Views.Common.EntityCell
{
    public class ContactCollectionDelegate : UICollectionViewDelegate
    {
        private readonly ContactCollectionSource _collectionSource;

        public ContactCollectionDelegate(ContactCollectionSource collectionSource)
        {
            _collectionSource = collectionSource;
        }

        public ICommand NavigateSiteLinkCommand { get; set; }

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

        public override void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var item = _collectionSource.ItemsSource.Cast<object>().ElementAt(indexPath.Row);

            var phoneInfo = item as PhoneInfo;
            if (phoneInfo != null)
            {
                if (phoneInfo.Phone != null)
                {
                    using (var url = new NSUrl("tel:" + phoneInfo.Phone))
                    {
                        UIApplication.SharedApplication.OpenUrl(url);
                    }
                }
            }

            var emailInfo = item as EmailInfo;
            if (emailInfo != null)
            {
                if (emailInfo.Email != null)
                {
                    using (var url = new NSUrl("mailto:" + emailInfo.Email))
                    {
                        UIApplication.SharedApplication.OpenUrl(url);
                    }
                }
            }

            var webSiteInfo = item as WebSiteInfo;
            if (webSiteInfo != null)
            {
                if (NavigateSiteLinkCommand != null &&
                    NavigateSiteLinkCommand.CanExecute(webSiteInfo))
                {
                    NavigateSiteLinkCommand.Execute(webSiteInfo);
                }
            }

            collectionView.DeselectItem(indexPath, false);
        }
    }
}