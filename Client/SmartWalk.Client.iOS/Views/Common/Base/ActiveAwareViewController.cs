using Cirrious.MvvmCross.Touch.Views;
using SmartWalk.Client.Core.ViewModels.Interfaces;

namespace SmartWalk.Client.iOS.Views.Common.Base
{
    public abstract class ActiveAwareViewController : MvxViewController
    {
        protected bool IsActive
        {
            get { return !(ViewModel is IActiveAware) || ((IActiveAware)ViewModel).IsActive; }
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            var activeViewModel = ViewModel as IActiveAware;
            if (activeViewModel != null)
            {
                activeViewModel.IsActive = true;
            }
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);

            var activeViewModel = ViewModel as IActiveAware;
            if (activeViewModel != null)
            {
                activeViewModel.IsActive = false;
            }
        }
    }
}