using System;
using UIKit;
using SmartWalk.Client.iOS.Utils;

namespace SmartWalk.Client.iOS.Views.Common.Base.Cells
{
    public abstract class TableCellBase : UITableViewCell
    {
        private object _dataContext;

        protected TableCellBase(IntPtr handle) : base(handle) 
        {
            // HACK: http://stackoverflow.com/questions/19132908/auto-layout-constraints-issue-on-ios7-in-uitableviewcell
            ContentView.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;
        }

        public object DataContext
        {
            get { return _dataContext; }
            set
            {
                if (!Equals(_dataContext, value))
                {
                    var previousContext = _dataContext;
                    _dataContext = value;
                    OnDataContextChanged(previousContext, _dataContext);
                }
            }
        }

        protected virtual void OnDataContextChanged(object previousContext, object newContext)
        {
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            ConsoleUtil.LogDisposed(this);
        }
    }
}