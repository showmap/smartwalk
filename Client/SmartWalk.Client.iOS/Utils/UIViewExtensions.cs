using System;
using System.Collections.Generic;
using CoreAnimation;
using UIKit;
using SmartWalk.Client.iOS.Resources;
using CoreGraphics;
using System.Drawing;

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

        public static void SetHidden(this UIView view, bool hidden, bool animated, 
            double duration = UIConstants.AnimationDuration)
        {
            if (animated)
            {
                UIView.Transition(
                    view,
                    duration,
                    UIViewAnimationOptions.TransitionCrossDissolve,
                    () => view.Hidden = hidden,
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
                        () => view.Alpha = 0,
                        () => {
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
                        });
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
                        () => subview.Alpha = 1);
                }
                else
                {
                    view.Add(subview);
                }
            }
        }

        // Default image is shown during loading
        public static bool ProgressEnded(this UIImageView imageView)
        {
            var result = imageView.Image == null ||
                imageView.Image.Size != Theme.DefaultImageSize;
            return result;
        }

        public static bool HasImage(this UIImageView imageView)
        {
            var result = imageView.Image != null &&
                imageView.Image.Size != Theme.DefaultImageSize &&
                imageView.Image.Size != Theme.ErrorImageSize;
            return result;
        }

        public static void UpdateLayout(this UITableView tableView)
        {
            CATransaction.Begin();

            CATransaction.CompletionBlock = () => {
                var tableSource = tableView.WeakDelegate as UITableViewSource;
                if (tableSource != null)
                {
                    // to run NavBar show logic, if table is at (0,0) offset
                    tableSource.DecelerationEnded(tableView);
                }
            };

            tableView.BeginUpdates();
            tableView.EndUpdates();

            CATransaction.Commit();
        }

        public static void UpdateConstraint(this UIView view, Action animationHandler, bool animated)
        {
            if (animated)
            {
                UIView.Animate(
                    UIConstants.AnimationDuration, 
                    () =>
                    {
                        animationHandler();
                        view.LayoutIfNeeded();
                    });
            }
            else
            {
                animationHandler();
            }
        }

        public static bool LocatedInView(this UIGestureRecognizer recognizer, 
            UIView view, CGRect bounds = default(CGRect))
        {
            var viewBounds = bounds == CGRect.Empty ? view.Bounds : bounds;
            var result = viewBounds.IntersectsWith(
                new CGRect(recognizer.LocationInView(view), CGSize.Empty));
            return result;
        }

        public static void MakeRound(this UIView view)
        {
            var maskWidth = view.Frame.Width;
            var maskHeight = view.Frame.Height;
            const int maskGap = 0;

            var path = UIBezierPath.FromOval(new CGRect(0, 0, maskWidth, maskHeight));
            var mask = new CAShapeLayer {
                Frame = new CGRect(maskGap, maskGap,
                    maskGap + maskWidth, 
                    maskGap + maskHeight),
                Path = path.CGPath
            };

            view.Layer.Mask = mask;
        }
    }
}