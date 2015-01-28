using System;
using Cirrious.MvvmCross.Binding.Touch.Views;
using Foundation;
using UIKit;
using SmartWalk.Client.Core.Model;
using SmartWalk.Client.Core.ViewModels;
using SmartWalk.Client.iOS.Controls;

namespace SmartWalk.Client.iOS.Views.HomeView
{
    public class HomeCollectionSource : MvxCollectionViewSource, IListViewSource
    {
        private readonly HomeViewModel _viewModel;

        public HomeCollectionSource(UICollectionView collectionView, HomeViewModel viewModel)
            : base(collectionView)
        {
            _viewModel = viewModel;

            collectionView.RegisterNibForCell(OrgCell.Nib, OrgCell.Key);
            collectionView.RegisterNibForSupplementaryView(
                HomeHeaderView.Nib, 
                UICollectionElementKindSection.Header, 
                HomeHeaderView.Key);
        }

        public event EventHandler DataReloaded;

        public override void ReloadData()
        {
            base.ReloadData();

            if (DataReloaded != null)
            {
                DataReloaded(this, EventArgs.Empty);
            }
        }

        public override nint NumberOfSections(UICollectionView collectionView)
        {
            return 1;
        }

        public override UICollectionReusableView GetViewForSupplementaryElement(
            UICollectionView collectionView, 
            NSString elementKind,
            NSIndexPath indexPath)
        {
            var header = 
                (HomeHeaderView)collectionView.DequeueReusableSupplementaryView(
                    UICollectionElementKindSection.Header,
                    HomeHeaderView.Key,
                    indexPath);
            header.Initialize();
            header.DataContext = _viewModel;
            return header;
        }

        protected override UICollectionViewCell GetOrCreateCellFor(
            UICollectionView collectionView, 
            NSIndexPath indexPath, 
            object item)
        {
            var cell = (OrgCell)collectionView.DequeueReusableCell(OrgCell.Key, indexPath);
            cell.DataContext = (OrgEvent)item;
            return cell;
        }
    }
}