using Cirrious.MvvmCross.Touch.Views;
using SmartWalk.Client.Core.ViewModels.Interfaces;

namespace SmartWalk.Client.iOS.Views.Common.Base
{
    public abstract class ActiveAwareViewController : MvxViewController
    {
        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

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