using System;
using MonoTouch.UIKit;
using SmartWalk.Client.Core.Utils;

namespace SmartWalk.Client.iOS.Views.Common
{
    public abstract class ContentViewBase : UIView
    {
        private bool _isInitialized;
        private object _dataContext;

        protected ContentViewBase(IntPtr handle) : base(handle) 
        {
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