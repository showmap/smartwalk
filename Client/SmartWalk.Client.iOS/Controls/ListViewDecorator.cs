using System;
using System.Collections;
using MonoTouch.UIKit;
using SmartWalk.Client.iOS.Utils;

namespace SmartWalk.Client.iOS.Controls
{
    public class ListViewDecorator : IDisposable
    {
        private readonly UITableView _tableView;
        private readonly UICollectionView _collectionView;

        private UIRefreshControl _refreshControl;
        private UITableViewController _tableController;

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

        public UIScrollView View
        {
            get
            {
                return (UIScrollView)_tableView ?? _collectionView;
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

        public UIRefreshControl RefreshControl
        {
            get
            {
                return _refreshControl;
            }
            set
            {
                if (_refreshControl != null)
                {
                    DisposeRefreshControl();
                }

                _refreshControl = value;

                if (_refreshControl != null)
                {
                    InitializeRefreshControl();
                }
            }
        }

        public static ListViewDecorator Create(UITableView tableView)
        {
            return tableView != null ? new ListViewDecorator(tableView) : null;
        }

        public static ListViewDecorator Create(UICollectionView collectionView)
        {
            return collectionView != null ? new ListViewDecorator(collectionView) : null;
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
            
        public void Dispose()
        {
            if (_tableController != null)
            {
                _tableController.RefreshControl = null;
                _tableController.Dispose();
            }

            DisposeRefreshControl();

            ConsoleUtil.LogDisposed(this);
        }

        private void InitializeRefreshControl()
        {
            if (_refreshControl != null)
            {
                if (_tableView != null)
                {
                    if (_tableController == null)
                    {
                        _tableController = new UITableViewController();
                        _tableController.TableView = _tableView;
                    }

                    _tableController.RefreshControl = _refreshControl;
                }

                if (_collectionView != null)
                {
                    _collectionView.AddSubview(_refreshControl);
                }
            }
        }

        private void DisposeRefreshControl()
        {
            if (_refreshControl != null)
            {
                _refreshControl.RemoveFromSuperview();
                _refreshControl.Dispose();
                _refreshControl = null;
            }
        }
    }

    public interface IListViewSource 
    {
        IEnumerable ItemsSource { get; }

        void ScrolledToTop(UIScrollView scrollView);
    }
}