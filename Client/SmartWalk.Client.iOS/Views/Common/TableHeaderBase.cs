using System;
using System.Linq;
using MonoTouch.UIKit;
using SmartWalk.Client.Core.Utils;

namespace SmartWalk.Client.iOS.Views.Common
{
    public class TableHeaderBase : UITableViewHeaderFooterView
    {
        private bool _isInitialized;
        private object _dataContext;

        protected TableHeaderBase(IntPtr handle) : base(handle)
        {
            // HACK: http://stackoverflow.com/questions/19132908/auto-layout-constraints-issue-on-ios7-in-uitableviewcell
            // HACK: getting contentView manually because ContentView property is always null
            var contentView = Subviews.FirstOrDefault(v => v.Bounds.Height > 1);
            if (contentView != null)
            {
                contentView.AutoresizingMask = 
                    UIViewAutoresizing.FlexibleWidth | 
                    UIViewAutoresizing.FlexibleHeight;
            }
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
