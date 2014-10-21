using System;
using System.Collections.Generic;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Client.iOS.Resources;

namespace SmartWalk.Client.iOS.Utils
{
    public static class UIViewExtensions
    {
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
                    UIConstants.AnimationDuration,
                    UIViewAnimationOptions.TransitionCrossDissolve,
                    new NSAction(() => view.Hidden = hidden),
                    null);
            }
            else
            {
                view.Hidden = hidden;
            }
        }

        public static void RemoveFromSuperview(
            this UIView view, 
            bool animated, 
            Action completeHandler = null, 
            Func<bool> cancelHandler = null)
        {
            if (view.Superview != null)
            {
                if (animated)
                {
                    view.Alpha = 1;

                    UIView.Animate(
                        UIConstants.AnimationDuration,
                        new NSAction(() => view.Alpha = 0),
                        new NSAction(() => {
                            // checking if remove operation was cancelled already
                            if (cancelHandler == null || !cancelHandler())
                            {
                                view.RemoveFromSuperview();
                                if (completeHandler != null) completeHandler();
                            }
                            else 
                            {
                                view.Alpha = 1;
                            }
                        }));
                }
                else
                {
                    view.RemoveFromSuperview();
                    if (completeHandler != null) completeHandler();
                }
            }
        }

        public static void Add(this UIView view, UIView subview, bool animated)
        {
            if (subview.Superview == null)
            {
                if (animated)
                {
                    subview.Alpha = 0;
                    view.Add(subview);

                    UIView.Animate(
                        UIConstants.AnimationDuration,
                        new NSAction(() => subview.Alpha = 1));
                }
                else
                {
                    view.Add(subview);
                }
            }
        }

        public static bool HasImage(this UIImageView imageView)
        {
            var result = imageView.Image != null &&
                imageView.Image.Size != Theme.DefaultImageSize &&
                imageView.Image.Size != Theme.ErrorImageSize;
            return result;
        }
    }
}