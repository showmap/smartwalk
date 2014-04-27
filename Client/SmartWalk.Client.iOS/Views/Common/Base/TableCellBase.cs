using System;
using MonoTouch.UIKit;
using SmartWalk.Client.iOS.Utils;

namespace SmartWalk.Client.iOS.Views.Common.Base
{
    public abstract class TableCellBase : UITableViewCell
    {
        private bool _isInitialized;
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

                    if (!_isInitialized)
                    {
                        OnInitialize();
                        _isInitialized = true;
                    }

                    OnDataContextChanged(previousContext, _dataContext);
                }
            }
        }

        protected virtual void OnInitialize()
        {
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