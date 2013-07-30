using System;
using MonoTouch.UIKit;
using Cirrious.MvvmCross.Binding.Touch.Views;

namespace SmartWalk.iOS.Views.Common
{
    public abstract class TableCellBase : MvxTableViewCell
    {
        private bool _isInitialized;

        protected TableCellBase(IntPtr handle) : base(handle) {}

        public new object DataContext
        {
            get { return base.DataContext; }
            set
            {
                if (!Equals(base.DataContext, value))
                {
                    base.DataContext = value;

                    if (!_isInitialized)
                    {
                        OnInitialize();
                        _isInitialized = true;
                    }

                    OnDataContextChanged();
                }
            }
        }

        public virtual void DidRotate(UIInterfaceOrientation fromInterfaceOrientation)
        {
        }

        protected virtual void OnInitialize()
        {
        }

        protected virtual void OnDataContextChanged()
        {
        }
    }
}