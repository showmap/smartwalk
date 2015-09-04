using System;
using UIKit;
using SmartWalk.Client.iOS.Utils;

namespace SmartWalk.Client.iOS.Views.Common.Base.Cells
{
    public abstract class CollectionCellBase : UICollectionViewCell
    {
        private object _dataContext;

        protected CollectionCellBase(IntPtr handle) : base(handle)
        {
            // HACK: http://stackoverflow.com/questions/19132908/auto-layout-constraints-issue-on-ios7-in-uitableviewcell
            ContentView.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;
        }

        public object DataContext
        {
            get { return _dataContext; }
            set
            {
                if (!Equals(_dataContext, value))
                {
                    _dataContext = value;
                    OnDataContextChanged();
                }
            }
        }

        protected virtual void OnDataContextChanged()
        {
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            ConsoleUtil.LogDisposed(this);
        }
    }
}