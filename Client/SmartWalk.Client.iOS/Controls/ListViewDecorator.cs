using MonoTouch.UIKit;
using System;
using System.Collections;

namespace SmartWalk.Client.iOS.Controls
{
    public class ListViewDecorator
    {
        private readonly UITableView _tableView;
        private readonly UICollectionView _collectionView;

        public ListViewDecorator(UITableView tableView)
        {
            if (tableView == null) throw new ArgumentNullException("tableView");
            _tableView = tableView;
        }

        public ListViewDecorator(UICollectionView collectionView)
        {
            if (collectionView == null) throw new ArgumentNullException("collectionView");
            _collectionView = collectionView;
        }

        public UIView View
        {
            get
            {
                return (UIView)_tableView ?? (UIView)_collectionView;
            }
        }

        public IListViewSource Source
        {
            get
            {
                if (_tableView != null)
                {
                    return (IListViewSource)_tableView.Source;
                }

                if (_collectionView != null)
                {
                    return (IListViewSource)_collectionView.WeakDataSource;
                }

                return null;
            }
            set
            {
                if (_tableView != null)
                {
                    _tableView.Source = (UITableViewSource)value;
                }

                if (_collectionView != null)
                {
                    _collectionView.Source = (UICollectionViewSource)value;
                }
            }
        }

        public void ReloadData()
        {
            if (_tableView != null)
            {
                _tableView.ReloadData();
            }

            if (_collectionView != null)
            {
                _collectionView.ReloadData();
            }
        }

        public void AddSubview(UIView view)
        {
            if (_tableView != null)
            {
                _tableView.AddSubview(view);
            }

            if (_collectionView != null)
            {
                _collectionView.AddSubview(view);
            }
        }

        public void AddGestureRecognizer(UIGestureRecognizer gestureRecognizer)
        {
            if (_tableView != null)
            {
                _tableView.AddGestureRecognizer(gestureRecognizer);
            }

            if (_collectionView != null)
            {
                _collectionView.AddGestureRecognizer(gestureRecognizer);
            }
        }

        public void RemoveGestureRecognizer(UIGestureRecognizer gestureRecognizer)
        {
            if (_tableView != null)
            {
                _tableView.RemoveGestureRecognizer(gestureRecognizer);
            }

            if (_collectionView != null)
            {
                _collectionView.RemoveGestureRecognizer(gestureRecognizer);
            }
        }
    }

    public interface IListViewSource 
    {
        IEnumerable ItemsSource { get; }
    }
}