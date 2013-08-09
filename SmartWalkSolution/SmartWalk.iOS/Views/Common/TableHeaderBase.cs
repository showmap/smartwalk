using System;
using MonoTouch.UIKit;

namespace SmartWalk.iOS.Views.Common
{
    public class TableHeaderBase : UITableViewHeaderFooterView
    {
        private bool _isInitialized;
        private object _dataContext;

        protected TableHeaderBase(IntPtr handle) : base(handle) {}

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
    }
}
