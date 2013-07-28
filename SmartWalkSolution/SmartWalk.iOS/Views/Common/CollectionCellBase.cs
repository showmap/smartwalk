using System;
using MonoTouch.UIKit;

namespace SmartWalk.iOS.Views.Common
{
    public abstract class CollectionCellBase<TDataContext> : UICollectionViewCell
        where TDataContext : class
    {
        private bool _isInitialized;
        private TDataContext _dataContext;

        protected CollectionCellBase(IntPtr handle) : base(handle) {}

        public TDataContext DataContext
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