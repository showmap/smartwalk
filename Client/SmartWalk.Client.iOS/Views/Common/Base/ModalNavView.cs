using System;
using UIKit;

namespace SmartWalk.Client.iOS.Views.Common.Base
{
    public class ModalNavView : UINavigationController, IModalView
    {
        public ModalNavView(IModalView view) : base((UIViewController)view)
        {
            view.ToHide += OnViewToHide;

            if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            {
                HidesBarsOnSwipe = true;
            }
        }

        public event EventHandler ToHide;

        public override UIStatusBarStyle PreferredStatusBarStyle()
        {
            return UIStatusBarStyle.LightContent;
        }

        ViewBase IModalView.PresentingViewController
        {
            get { return (ViewBase)PresentingViewController; }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            var view = TopViewController as IModalView;
            if (view != null)
            {
                view.ToHide -= OnViewToHide;
                view.Dispose();
            }
        }

        private void OnViewToHide(object sender, EventArgs e)
        {
            if (ToHide != null)
            {
                ToHide(sender, e);
            }
        }
    }
}