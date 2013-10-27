using Cirrious.MvvmCross.Dialog.Touch;
using SmartWalk.Core.ViewModels.Interfaces;

namespace SmartWalk.iOS.Views.Common
{
    public abstract class ActiveAwareDialogController : MvxDialogViewController
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