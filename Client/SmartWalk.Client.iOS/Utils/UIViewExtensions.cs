using System;
using System.Collections.Generic;
using CoreAnimation;
using CoreGraphics;
using SmartWalk.Client.iOS.Resources;
using UIKit;
using ImageState = Cirrious.MvvmCross.Plugins.DownloadCache.MvxDynamicImageHelper<UIKit.UIImage>.ImageState;

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

        public static bool IsChildOf<T>(this UIView view) where T : UIView
        {
            return view.Superview != null &&
                (view.Superview is T || 
                    view.Superview.IsChildOf<T>());
        }

        public static T ParentOfType<T>(this UIView view) where T : UIView
        {
            return view.Superview == null 
                ? null 
                : view.Superview is T ? (T)view.Superview : view.Superview.ParentOfType<T>();
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

        public static bool ProgressEnded(this UIImageView imageView, ImageState state)
        {
            var result = state != ImageState.DefaultShown;
            return result;
        }

        public static bool HasImage(this UIImageView imageView)
        {
            var result = imageView != null &&
                imageView.Image != null &&
                imageView.Image.Size != Theme.DefaultImageSize &&
                imageView.Image.Size != Theme.ErrorImageSize;
            return result;
        }

        public static bool HasImage(this UIImageView imageView, ImageState state)
        {
            var result = imageView != null &&
                imageView.Image != null &&
                state != ImageState.DefaultShown &&
                state != ImageState.ErrorShown;
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

        public static void UpdateConstraint(this UIView view, 
            Action animationHandler, bool animated, Action completion = null)
        {
            Animate(() =>
                {
                    animationHandler();
                    view.LayoutIfNeeded();
                },
                animated,
                completion);
        }

        public static void Animate(Action animation, bool animated, Action completion = null)
        {
            if (animated)
            {
                UIView.Animate(UIConstants.AnimationDuration, animation, completion);
            }
            else
            {
                animation();
                if (completion != null) completion();
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

        public static void MakeRound(this UIView view, CGSize? size = null)
        {
            view.MakeMask(UIBezierPath.FromOval, size);
        }

        public static void MakeRect(this UIView view, CGSize? size = null)
        {
            view.MakeMask(UIBezierPath.FromRect, size);
        }

        private static void MakeMask(this UIView view, 
            Func<CGRect, UIBezierPath> getPath, 
            CGSize? size = null)
        {
            var maskWidth = !size.HasValue ? view.Frame.Width : size.Value.Width;
            var maskHeight = !size.HasValue ? view.Frame.Height : size.Value.Height;
            const int maskGap = 0;

            var path = getPath(new CGRect(0, 0, maskWidth, maskHeight));
            var frame = new CGRect(maskGap, maskGap,
                maskGap + maskWidth,  
                maskGap + maskHeight);

            var maskLayer = view.Layer.Mask as CAShapeLayer;
            if (maskLayer != null)
            {
                maskLayer.Frame = frame;
                maskLayer.Path = path.CGPath;
            }
            else
            {
                var mask = new CAShapeLayer {
                    Frame = frame,
                    Path = path.CGPath
                };

                view.Layer.Mask = mask;
            }
        }
    }
}