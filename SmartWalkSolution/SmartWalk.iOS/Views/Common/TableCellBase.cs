using Cirrious.MvvmCross.Binding.Touch.Views;
using System;

namespace SmartWalk.iOS.Views.Common
{
    public abstract class TableCellBase : MvxTableViewCell
    {
        private bool _isInitialized;

        protected TableCellBase(IntPtr handle) : base(handle) {}

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