using System;
using UIKit;
using SmartWalk.Client.iOS.Utils;

namespace SmartWalk.Client.iOS.Views.Common.Base.Cells
{
    public abstract class ContentViewBase : UIView
    {
        private object _dataContext;

        protected ContentViewBase() {}

        protected ContentViewBase(IntPtr handle) : base(handle) {}

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