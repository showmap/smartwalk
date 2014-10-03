using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace SmartWalk.Client.iOS.Utils
{
    public class ExtendedNavController : UINavigationController
    {
        private UINavigationItem _poped;

        public ExtendedNavController(UIViewController root) : base(root)
        {
            WeakDelegate = this;
        }

        public override void PushViewController(UIViewController viewController, bool animated)
        {
            base.PushViewController(viewController, animated);

            NavBarManager.Instance.NavBar.PushNavigationItem(viewController.NavigationItem, animated);
        }

        public override UIViewController PopViewControllerAnimated(bool animated)
        {
            var result = base.PopViewControllerAnimated(animated);

            _poped = NavBarManager.Instance.NavBar.PopNavigationItemAnimated(animated);

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
                        NavBarManager.Instance.NavBar
                            .PushNavigationItem(_poped, context.IsAnimated);
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
    }
}

