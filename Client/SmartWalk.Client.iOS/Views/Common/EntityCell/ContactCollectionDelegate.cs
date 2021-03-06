using System.Linq;
using System.Windows.Input;
using Foundation;
using UIKit;
using SmartWalk.Client.Core.Model.DataContracts;
using SmartWalk.Shared.DataContracts;
using SmartWalk.Client.iOS.Resources;
using SmartWalk.Client.Core.ViewModels.Interfaces;

namespace SmartWalk.Client.iOS.Views.Common.EntityCell
{
    public class ContactCollectionDelegate : UICollectionViewDelegate
    {
        private readonly ContactCollectionSource _collectionSource;

        public ContactCollectionDelegate(ContactCollectionSource collectionSource)
        {
            _collectionSource = collectionSource;
        }

        public ICommand CallPhoneCommand { get; set; }
        public ICommand ComposeEmailCommand { get; set; }
        public ICommand ShowHideModalViewCommand { get; set; }

        public override void ItemHighlighted(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = collectionView.CellForItem(indexPath);
            cell.ContentView.BackgroundColor = ThemeColors.ContentLightHighlight;
        }

        public override void ItemUnhighlighted(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = collectionView.CellForItem(indexPath);
            cell.ContentView.BackgroundColor = null;
        }

        public override void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var item = _collectionSource.ItemsSource.Cast<object>().ElementAt(indexPath.Row);

            var contact = item as Contact;
            if (contact != null)
            {
                if (contact.Type == ContactType.Phone)
                {
                    if (CallPhoneCommand != null &&
                        CallPhoneCommand.CanExecute(contact))
                    {
                        CallPhoneCommand.Execute(contact);
                    }
                }

                if (contact.Type == ContactType.Email)
                {
                    if (ComposeEmailCommand != null &&
                        ComposeEmailCommand.CanExecute(contact))
                    {
                        ComposeEmailCommand.Execute(contact);
                    }
                }

                if (contact.Type == ContactType.Url)
                {
                    var context = new ModalViewContext(
                        ModalViewKind.Browser, 
                        contact.ContactText);
                    
                    if (ShowHideModalViewCommand != null &&
                        ShowHideModalViewCommand.CanExecute(context))
                    {
                        ShowHideModalViewCommand.Execute(context);
                    }
                }
            }

            collectionView.DeselectItem(indexPath, false);
        }
    }
}