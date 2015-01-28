using System;
using System.Collections.Generic;
using System.Linq;
using UIKit;
using SmartWalk.Shared.Utils;
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

        public static float NavBarHeight
        {
            get
            { 
                var result = 
                    ScreenUtil.IsVerticalOrientation
                        ? UIConstants.ToolBarVerticalHeight
                        : UIConstants.ToolBarHorizontalHeight;

                return result + UIConstants.StatusBarHeight;
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

        private TransparentNavBar _navBar;
        private NSLayoutConstraint[] _navBarConstraints;
        private NSLayoutConstraint _navBarHeightConstraint;
        private bool _hidden = true;

        private NavBarManager()
        {
            Initialize();
        }

        public UINavigationBar NavBar
        {
            get { return _navBar; }
        }

        public bool IsTransparent
        {
            get { return _navBar.IsTransparent; }
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
                            "H:|-0-[navBar]-0-|", 0,
                            "navBar", 
                            _navBar));

                    var vertical =
                        NSLayoutConstraint.FromVisualFormat(
                            "V:|-0-[navBar(navBarHeight)]", 0,
                            "navBarHeight",
                            NavBarHeight,
                            "navBar", 
                            _navBar);

                    _navBarHeightConstraint = vertical
                        .FirstOrDefault(c => c.Constant.EqualsNF(NavBarHeight));

                    constraints.AddRange(vertical);

                    _navBarConstraints = constraints.ToArray();
                }

                return _navBarConstraints;
            }
        }

        public void Layout()
        {
            UpdateNavBarConstraints();
            ButtonBarUtil.UpdateButtonsFrameOnRotation(_navBar.GetNavBarItems());
        }

        public void SetHidden(bool hidden, bool animated)
        {
            if (_hidden != hidden)
            {
                _hidden = hidden;
           
                if (hidden)
                {
                    _navBar.RemoveFromSuperview(
                        animated, 
                        () => NavController.View.RemoveConstraints(NavBarConstraints),
                        () => !_hidden);
                }
                else
                {
                    NavController.View.Add(_navBar, animated);
                    NavController.View.AddConstraints(NavBarConstraints);
                }
            }
        }

        public void SetTransparentType(SemiTransparentType transparentType, bool animated)
        {
            _navBar.SetTransparent(transparentType != SemiTransparentType.None, animated);
            _navBar.ItemSemiTransparentType = transparentType;
        }

        private void Initialize()
        {
            _navBar = new TransparentNavBar
                {
                    IsTransparent = true,
                };

            _navBar.TranslatesAutoresizingMaskIntoConstraints = false;
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