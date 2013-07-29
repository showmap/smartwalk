using System;
using MonoTouch.UIKit;

namespace SmartWalk.iOS.Views.Common
{
    public abstract class TableCellBase : UITableViewCell
    {
        private bool _isInitialized;
        private object _dataContext;

        protected TableCellBase(IntPtr handle) : base(handle) {}

        public object DataContext
        {
            get { return _dataContext; }
            set
            {
                if (!Equals(_dataContext, value))
                {
                    _dataContext = value;

                    if (!_isInitialized)
                    {
                        OnInitialize();
                        _isInitialized = true;
                    }

                    OnDataContextChanged();
                }
            }
        }

        protected virtual void OnInitialize()
        {
        }

        protected virtual void OnDataContextChanged()
        {
        }
    }
}