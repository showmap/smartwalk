using System;
using System.Drawing;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Client.iOS.Resources;
using SmartWalk.Client.iOS.Utils;

namespace SmartWalk.Client.iOS.Controls
{
    public class TransparentNavBar : UINavigationBar
    {
        private bool _isTransparent;
        private bool _isItemSemiTransparent = true;

        public TransparentNavBar()
        {
            Initialize();
        }

        public bool IsTransparent
        {
            get
            {
                return _isTransparent;
            }
            set
            {
                SetTransparent(value, false);
            }
        }

        public bool IsItemSemiTransparent
        {
            get
            {
                return _isItemSemiTransparent;
            }
            set
            {
                if (_isItemSemiTransparent != value)
                {
                    _isItemSemiTransparent = value;
                    UpdateItemsState();
                }
            }
        }

        public void SetTransparent(bool transparent, bool animated)
        {
            if (_isTransparent != transparent)
            {
                _isTransparent = transparent;
                UpdateState(animated);
            }
        }

        public override UIView HitTest(PointF point, UIEvent uievent)
        {
            if (IsTransparent)
            {
                var view = base.HitTest(point, uievent);
                var rootView = GetRootView();
                return view == this && rootView != null
                    ? rootView.HitTest(point, uievent) 
                    : view;
            }

            return base.HitTest(point, uievent);
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            // manually adjusting position of items in landscape (stick it to bottom)
            if (!ScreenUtil.IsVerticalOrientation)
            {
                var itemViews = ButtonBarUtil.GetViewsFromBarItems(TopItem.GetNavItemBarItems());

                foreach (var itemView in itemViews)
                {
                    var frame = new RectangleF(
                        new PointF(itemView.Frame.X, Frame.Height - itemView.Frame.Height),
                        itemView.Frame.Size);
                    itemView.Frame = frame;
                }
            }
        }

        private void Initialize()
        {
            Translucent = true;
            UpdateState(false);
        }

        private void UpdateState(bool animated)
        {
            Action action;

            if (IsTransparent)
            {
                SetBackgroundImage(new UIImage(), UIBarMetrics.Default);
                ShadowImage = new UIImage();

                action = new Action(() =>
                {
                    BarTintColor = UIColor.Clear;
                });
            }
            else
            {
                SetBackgroundImage(null, UIBarMetrics.Default);
                ShadowImage = new UIImage();

                action = new Action(() =>
                {
                    BarTintColor = Theme.NavBarBackgroundiOS7;
                });
            }

            if (animated)
            {
                UIView.Animate(UIConstants.AnimationDuration, new NSAction(action));
            }
            else
            {
                action();
            }
        }

        private void UpdateItemsState()
        {
            var buttons = ButtonBarUtil.GetButtonsFromBarItems(TopItem.GetNavItemBarItems());
            foreach (var button in buttons)
            {
                button.IsSemiTransparent = IsItemSemiTransparent;
            }
        }

        private UIView GetRootView()
        {
            var result = Window.RootViewController.View.Subviews
                .FirstOrDefault(sv => sv != this);
            return result;
        }
    }
}

