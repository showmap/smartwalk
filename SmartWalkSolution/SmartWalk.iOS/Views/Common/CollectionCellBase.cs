using System;
using MonoTouch.UIKit;
using SmartWalk.Core.Utils;

namespace SmartWalk.iOS.Views.Common
{
    public abstract class CollectionCellBase : UICollectionViewCell
    {
        private bool _isInitialized;
        private object _dataContext;

        protected CollectionCellBase(IntPtr handle) : base(handle) {}

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

        /*protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            ConsoleUtil.LogDisposed(this);
        }*/
    }
}