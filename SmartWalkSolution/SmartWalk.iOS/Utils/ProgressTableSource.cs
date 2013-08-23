using MonoTouch.UIKit;
using Cirrious.MvvmCross.Binding.Touch.Views;

namespace SmartWalk.iOS.Utils
{
    public class ProgressTableSource : MvxTableViewSource
    {
        private bool _isLoading;

        protected ProgressTableSource(UITableView tableView) : base(tableView)
        {
        }

        public bool IsProgressVisible
        {
            get
            {
                return ItemsSource == null && IsLoading;
            }
        }

        public bool IsLoading
        {
            get
            {
                return _isLoading;
            }
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;

                    if (IsProgressVisible)
                    {
                        ReloadTableData();
                    }
                }
            }
        }

        protected override UITableViewCell GetOrCreateCellFor(UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath, object item)
        {
            return null;
        }
    }
}