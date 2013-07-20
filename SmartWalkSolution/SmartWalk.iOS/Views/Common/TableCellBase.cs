using System;
using Cirrious.MvvmCross.Binding.Binders;
using Cirrious.MvvmCross.Binding.Touch.Views;

namespace SmartWalk.iOS.Views.Common
{
    public abstract class TableCellBase : MvxTableViewCell
    {
        private bool _isInitialized;

        protected TableCellBase(MvxBindingDescription[] bindings) : base(bindings)
        {    
        }

        protected TableCellBase(MvxBindingDescription[] bindings, IntPtr handle) : base(bindings, handle)
        {
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            if (!_isInitialized)
            {
                if (Initialize())
                {
                    _isInitialized = true;
                }
            }
        }

        protected virtual bool Initialize()
        {
            return true;
        }
    }
}