using System.Drawing;
using Cirrious.MvvmCross.Binding.Touch.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Client.Core.Model;
using SmartWalk.Client.iOS.Controls;

namespace SmartWalk.Client.iOS.Views.HomeView
{
    public class HomeCollectionSource : MvxCollectionViewSource, IListViewSource
    {
        public HomeCollectionSource(UICollectionView collectionView)
            : base(collectionView)
        {
            collectionView.RegisterNibForCell(OrgCell.Nib, OrgCell.Key);
        }

        public override void Scrolled(UIScrollView scrollView)
        {
            UIApplication.SharedApplication.SetStatusBarHidden(
                scrollView.ContentOffset != PointF.Empty, 
                UIStatusBarAnimation.Slide);
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