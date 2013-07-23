using System;
using Cirrious.MvvmCross.Binding.Touch.Views;

namespace SmartWalk.iOS.Views.Common
{
    public abstract class CollectionCellBase : MvxCollectionViewCell
    {
        private bool _isInitialized;

        protected CollectionCellBase(IntPtr handle) : base(handle) {}

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