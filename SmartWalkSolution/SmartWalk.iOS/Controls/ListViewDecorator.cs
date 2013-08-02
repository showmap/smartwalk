using MonoTouch.UIKit;

namespace SmartWalk.iOS.Controls
{
    public class ListViewDecorator
    {
        private readonly UITableView _tableView;
        private readonly UICollectionView _collectionView;

        public ListViewDecorator(UITableView tableView)
        {
            _tableView = tableView;
        }

        public ListViewDecorator(UICollectionView collectionView)
        {
            _collectionView = collectionView;
        }

        public object Source
        {
            get
            {
                if (_tableView != null)
                {
                    return _tableView.Source;
                }

                if (_collectionView != null)
                {
                    return _collectionView.Source;
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
    }
}