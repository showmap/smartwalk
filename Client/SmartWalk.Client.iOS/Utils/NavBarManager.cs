using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
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
            get
            { 
                return AppDelegate.Window != null 
                    ? (UINavigationController)AppDelegate.Window.RootViewController 
                    : null;
            }
        }

        private static float NavBarHeight
        {
            get
            {
                var result = 
                    ScreenUtil.IsVerticalOrientation
                        ? UIConstants.ToolBarVerticalHeight
                        : UIConstants.ToolBarHorizontalHeight;

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

        private TransparentToolBar _customNavBar;
        private NSLayoutConstraint[] _navBarConstraints;
        private NSLayoutConstraint _navBarHeightConstraint;
        private bool _nativeHidden;
        private bool _customHidden = true;

        private NavBarManager()
        {
            InitializeBars();
        }

        public bool NativeHidden
        {
            get { return _nativeHidden; }
        }

        public bool CustomHidden
        {
            get { return _customHidden; }
        }

        private NSLayoutConstraint[] NavBarConstraints
        {
            get
            {
                if (_navBarConstraints == null)
                {
                    var constraints = new List<NSLayoutConstraint>();

                    constraints.AddRange(
                        NSLayoutConstraint.FromVisualFormat(
                            "H:|-0-[navBar]-0-|", 
                            0,
                            null, 
                            new NSDictionary(
                                "navBar", 
                                _customNavBar)));

                    var vertical =
                        NSLayoutConstraint.FromVisualFormat(
                            "V:|-(statusBarHeight)-[navBar(navBarHeight)]", 
                            0, 
                            new NSDictionary(
                                "statusBarHeight", 
                                UIConstants.StatusBarHeight,
                                "navBarHeight",
                                NavBarHeight),
                            new NSDictionary(
                                "navBar", 
                                _customNavBar));

                    _navBarHeightConstraint = vertical
                        .FirstOrDefault(c => Math.Abs(c.Constant - NavBarHeight) < UIConstants.Epsilon);

                    constraints.AddRange(vertical);

                    _navBarConstraints = constraints.ToArray();
                }

                return _navBarConstraints;
            }
        }

        public void Layout()
        {
            UpdateNavBarConstraints();
            ButtonBarUtil.UpdateButtonsFrameOnRotation(_customNavBar.Items);
        }

        public void SetNavBarHidden(
            bool nativeHidden, 
            bool customHidden, 
            bool animated)
        {
            _nativeHidden = nativeHidden;
            NavController.SetNavigationBarHidden(nativeHidden, animated);

            if (_customHidden != customHidden)
            {
                _customHidden = customHidden;
           
                if (customHidden)
                {
                    _customNavBar.RemoveFromSuperview(
                        animated, 
                        () => NavController.View.RemoveConstraints(NavBarConstraints),
                        () => !_customHidden);
                }
                else
                {
                    NavController.View.Add(_customNavBar, animated);
                    NavController.View.AddConstraints(NavBarConstraints);
                }
            }
        }

        public void SetCustomItems(UIBarButtonItem[] items, bool animated)
        {
            _customNavBar.SetItems(items, animated);
        }

        private void InitializeBars()
        {
            _customNavBar = new TransparentToolBar();
            _customNavBar.Translucent = true;
            _customNavBar.BackgroundColor = UIColor.Clear;
            _customNavBar.TranslatesAutoresizingMaskIntoConstraints = false;
        }

        private void UpdateNavBarConstraints()
        {
            if (_navBarHeightConstraint != null)
            {
                _navBarHeightConstraint.Constant = NavBarHeight;
            }
        }
    }
}