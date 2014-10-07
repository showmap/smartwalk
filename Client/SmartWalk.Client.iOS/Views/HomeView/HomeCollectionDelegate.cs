using System;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Client.Core.Model;
using SmartWalk.Client.Core.ViewModels;
using SmartWalk.Client.iOS.Resources;
using SmartWalk.Client.iOS.Utils;

namespace SmartWalk.Client.iOS.Views.HomeView
{
    public class HomeCollectionDelegate : UICollectionViewDelegate
    {
        private readonly UICollectionView _collectionView;
        private readonly HomeViewModel _viewModel;
        private readonly HomeCollectionSource _collectionSource;

        private bool _isTouched;

        public HomeCollectionDelegate(
            UICollectionView collectionView, 
            HomeViewModel viewModel, 
            HomeCollectionSource collectionSource)
        {
            _collectionView = collectionView;
            _viewModel = viewModel;
            _collectionSource = collectionSource;
            _collectionSource.DataReloaded += OnDataReloaded;
        }

        public bool IsHeaderViewHidden
        {
            get
            {
                return _collectionView.ActualContentOffset() >= 
                    HomeHeaderView.DefaultHeight;
            }
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

        public void ScrollOutHeader()
        {
            ScrollUtil.ScrollOutHeader(
                _collectionView, 
                HomeHeaderView.DefaultHeight, 
                _isTouched);
        }

        public override void DraggingStarted(UIScrollView scrollView)
        {
            _isTouched = true;
        }

        public override void DraggingEnded(UIScrollView scrollView, bool willDecelerate)
        {
            ScrollUtil.AdjustHeaderPosition(scrollView, HomeHeaderView.DefaultHeight, true);
        }

        protected override void Dispose(bool disposing)
        {
            if (_collectionSource != null)
            {
                _collectionSource.DataReloaded -= OnDataReloaded;
            }

            base.Dispose(disposing);
            ConsoleUtil.LogDisposed(this);
        }

        private void OnDataReloaded(object sender, EventArgs e)
        {
            if (!IsHeaderViewHidden)
            {
                ScrollUtil.ScrollOutHeaderAfterReload(
                    _collectionView, 
                    HomeHeaderView.DefaultHeight, 
                    _collectionSource, 
                    _isTouched);
            }
        }
    }
}