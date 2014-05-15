using System.Drawing;
using MonoTouch.UIKit;
using SmartWalk.Client.iOS.Controls;
using SmartWalk.Client.iOS.Utils;

namespace SmartWalk.Client.iOS.Utils
{
    public sealed class NavBarManager
    {
        private static NavBarManager _instance;

        private static UINavigationController NavController 
        {
            get { return (UINavigationController)AppDelegate.Window.RootViewController; }
        }

        private static float StatusBarHeight
        {
            get
            {
                float result;

                if (!UIApplication.SharedApplication.StatusBarFrame.IsEmpty)
                {
                    result = 
                        ScreenUtil.IsVerticalOrientation 
                            ? UIApplication.SharedApplication.StatusBarFrame.Height 
                            : UIApplication.SharedApplication.StatusBarFrame.Width;
                }
                else
                {
                    result = 20;
                }

                return result;
            }
        }

        public static NavBarManager Instance 
        { 
            get
            {
                if (_instance == null)
                {
                    _instance = new NavBarManager();
                }

                return _instance;
            }
        }

        private NavBarManager()
        {
            InitializeBars();
        }

        public TransparentToolBar CustomNavBar { get; private set; }

        public void Layout()
        {
            UpdateCustomNavBarFrame();
            ButtonBarUtil.UpdateButtonsFrameOnRotation(CustomNavBar.Items);
        }

        public void SetNavBarVisibility(
            bool isNativeVisible, 
            bool isCustomVisible, 
            bool animated)
        {
            NavController.SetNavigationBarHidden(!isNativeVisible, animated);

            UpdateCustomNavBarFrame();
           
            if (isCustomVisible)
            {
                if (CustomNavBar.Superview == null)
                {
                    NavController.View.Add(CustomNavBar);
                }
            }
            else
            {
                if (CustomNavBar.Superview != null)
                {
                    CustomNavBar.RemoveFromSuperview();
                }
            }
        }

        private void InitializeBars()
        {
            CustomNavBar = new TransparentToolBar();
            CustomNavBar.Translucent = true;
            CustomNavBar.BackgroundColor = UIColor.Clear;

            UpdateCustomNavBarFrame();
        }

        private void UpdateCustomNavBarFrame()
        {
            CustomNavBar.Frame = ScreenUtil.IsVerticalOrientation
                ? new RectangleF(0, StatusBarHeight, AppDelegate.Window.Bounds.Width, 44)
                : new RectangleF(0, StatusBarHeight, AppDelegate.Window.Bounds.Height, 33);
        }
    }
}