using UIKit;
using Foundation;

namespace SmartWalk.Client.iOS.Utils
{
    public class ExtendedNavController : UINavigationController
    {
        private UINavigationItem _poped;

        public ExtendedNavController(UIViewController root) : base(root)
        {
            WeakDelegate = this;
        }

        private static UINavigationBar NavBar
        {
            get { return NavBarManager.Instance.NavBar; }
        }

        public override void PushViewController(UIViewController viewController, bool animated)
        {
            base.PushViewController(viewController, animated);

            NavBar.PushNavigationItem(viewController.NavigationItem, animated);
        }

        public override UIViewController PopViewController(bool animated)
        {
            var result = base.PopViewController(animated);

            _poped = NavBar.PopNavigationItem(animated);

            return result;
        }

        [Export("navigationController:willShowViewController:animated:")]
        public void WillShowViewController(
            UINavigationController navigationController, 
            UIViewController viewController, 
            bool animated)
        {
            var coordinator = viewController.GetTransitionCoordinator();
            if (coordinator != null)
            {
                coordinator.NotifyWhenInteractionEndsUsingBlock(context => 
                {
                    if (context.IsCancelled && _poped != null)
                    {
                        NavBar.PushNavigationItem(_poped, context.IsAnimated);
                        Maintenance();
                    }

                    _poped = null;
                });
            }
        }

        [Export("navigationController:didShowViewController:animated:")]
        public void DidShowViewController(
            UINavigationController navigationController, 
            UIViewController viewController, 
            bool animated)
        {
            InteractivePopGestureRecognizer.Enabled = true;
            InteractivePopGestureRecognizer.WeakDelegate = viewController;
        }

        // HACK: To remove the twicely added last item upon animation failure 
        private static void Maintenance()
        {
            if (NavBar.TopItem == NavBar.BackItem)
            {
                NavBar.PopNavigationItem(false);
            }
        }
    }
}

