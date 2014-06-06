using MonoTouch.UIKit;
using System.Collections.Generic;
using MonoTouch.Foundation;

namespace SmartWalk.Client.iOS.Utils
{
    public static class UIViewExtensions
    {
        public const double DefaultAnimationDuration = 0.15;

        public static IEnumerable<UIView> GetAllSubViews(this UIView view)
        {
            var result = new List<UIView>();
            result.AddRange(view.Subviews);

            foreach (var subview in view.Subviews)
            {
                result.AddRange(subview.GetAllSubViews());
            }

            return result;
        }

        public static void RemoveSubviews(this UIView view)
        {
            foreach (var subView in view.Subviews)
            {
                subView.RemoveFromSuperview();
            }
        }

        public static bool IsChildOf<T>(this UIView view)
        {
            return view.Superview != null &&
                (view.Superview is T || 
                    view.Superview.IsChildOf<T>());
        }

        public static void SetHidden(this UIView view, bool hidden, bool animated)
        {
            if (animated)
            {
                UIView.Transition(
                    view,
                    DefaultAnimationDuration,
                    UIViewAnimationOptions.TransitionCrossDissolve,
                    new NSAction(() => view.Hidden = hidden),
                    null);
            }
            else
            {
                view.Hidden = hidden;
            }
        }
    }
}