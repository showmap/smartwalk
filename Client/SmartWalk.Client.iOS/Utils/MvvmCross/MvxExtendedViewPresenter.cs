using Cirrious.CrossCore.Platform;
using Cirrious.MvvmCross.Touch.Views;
using Cirrious.MvvmCross.Touch.Views.Presenters;
using Cirrious.MvvmCross.ViewModels;
using UIKit;
using SmartWalk.Client.Core.Services;
using SmartWalk.Client.Core.ViewModels;
using SmartWalk.Client.iOS.Views.Common;

namespace SmartWalk.Client.iOS.Utils.MvvmCross
{
    public class MvxExtendedViewPresenter : MvxTouchViewPresenter, IMvxExtendedViewPresenter
    {
        public MvxExtendedViewPresenter(
            IUIApplicationDelegate applicationDelegate, 
            UIWindow window) : base(applicationDelegate, window)
        {
        }

        public void HideDialogs()
        {
            if (MasterNavigationController.TopViewController != 
                MasterNavigationController.VisibleViewController)
            {
                var modalView = MasterNavigationController.VisibleViewController as IModalView;
                if (modalView != null)
                {
                    modalView.PresentingViewController.ShowHideModalView(null);
                }

                MasterNavigationController.VisibleViewController
                    .DismissViewController(false, null);
            }
        }

        public void ShowRoot()
        {
            if (!(GetCurrentViewModel() is HomeViewModel))
            {
                MasterNavigationController.PopToRootViewController(false);
            }
        }

        protected override UINavigationController CreateNavigationController(UIViewController viewController)
        {
            var result = new ExtendedNavController(viewController);
            return result;
        }

        private IMvxViewModel GetCurrentViewModel()
        {
            if (MasterNavigationController.TopViewController == null)
            {
                MvxTrace.Warning("Don't know how to get current viewmodel - no topmost", new object[0]);
            }
            else
            {
                var mvxTouchView = MasterNavigationController.TopViewController as IMvxTouchView;
                if (mvxTouchView == null)
                {
                    MvxTrace.Warning("Don't know how to get current viewmodel - topmost is not a touchview", new object[0]);
                }
                else
                {
                    return mvxTouchView.ViewModel;
                }
            }

            return null;
        }
    }
}