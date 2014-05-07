using System.Drawing;
using MonoTouch.UIKit;
using SmartWalk.Client.iOS.Utils;
using SmartWalk.Client.iOS.Controls;

namespace SmartWalk.Client.iOS.Utils
{
    public sealed class NavBarManager
    {
        public static NavBarManager Instance { get; private set; }

        internal static void Initialize(UIWindow window)
        {
            if (Instance == null)
            {
                Instance = new NavBarManager(window);
            }
        }

        private NavBarManager(UIWindow window)
        {
            Window = window;

            InitializeBars();
        }

        public UIWindow Window { get; private set; }
        public UIToolbar NavBar { get; private set; }

        private UINavigationController NavController 
        {
            get { return (UINavigationController)Window.RootViewController; }
        }

        public void Rotate()
        {
            UpdateFrames();
            ButtonBarUtil.UpdateButtonsFrameOnRotation(NavBar.Items);
        }

        public void SetNavBarVisibility(
            bool isStatusVisible,
            bool isNativeVisible, 
            bool isCustomVisible, 
            bool animated)
        {
            UIApplication.SharedApplication.SetStatusBarHidden(
                !isStatusVisible, 
                animated ? UIStatusBarAnimation.Slide : UIStatusBarAnimation.None);

            NavController.SetNavigationBarHidden(!isNativeVisible, animated);
           
            if (isCustomVisible)
            {
                if (NavBar.Superview == null)
                {
                    Window.RootViewController.View.Add(NavBar);
                }
            }
            else
            {
                if (NavBar.Superview != null)
                {
                    NavBar.RemoveFromSuperview();
                }
            }
        }

        private void InitializeBars()
        {
            NavBar = new TransparentToolBar();

            UpdateFrames();
        }

        private void UpdateFrames()
        {
            var statusBarFrame = UIApplication.SharedApplication.StatusBarFrame;
            NavBar.Frame = ScreenUtil.IsVerticalOrientation
                ? new RectangleF(0, statusBarFrame.Height, Window.Bounds.Width, 44)
                : new RectangleF(0, statusBarFrame.Width, Window.Bounds.Height, 33);
        }
    }
}