using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Client.Core.Model;
using SmartWalk.Client.Core.ViewModels;
using SmartWalk.Client.iOS.Resources;

namespace SmartWalk.Client.iOS.Views.HomeView
{
    public class HomeCollectionDelegate : UICollectionViewDelegate
    {
        private readonly HomeViewModel _viewModel;
        private readonly HomeCollectionSource _collectionSource;

        public HomeCollectionDelegate(HomeViewModel viewModel, HomeCollectionSource collectionSource)
        {
            _viewModel = viewModel;
            _collectionSource = collectionSource;
        }

        public override void ItemHighlighted(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = (OrgCell)collectionView.CellForItem(indexPath);
            cell.ContentView.BackgroundColor = Theme.CellHighlight;
        }

        public override void ItemUnhighlighted(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = (OrgCell)collectionView.CellForItem(indexPath);
            cell.ContentView.BackgroundColor = null;
        }

        public override void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var eventInfo = _collectionSource.ItemsSource.Cast<OrgEvent>().ElementAt(indexPath.Row);

            if (_viewModel.NavigateOrgEventViewCommand.CanExecute(eventInfo))
            {
                _viewModel.NavigateOrgEventViewCommand.Execute(eventInfo);
            }

            collectionView.DeselectItem(indexPath, false);
        }
    }
}