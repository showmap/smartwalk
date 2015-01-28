using System;
using System.Collections.Generic;
using CoreGraphics;
using System.Linq;
using Foundation;
using UIKit;
using SmartWalk.Client.iOS.Controls;

namespace SmartWalk.Client.iOS.Utils
{
    public static class ScrollUtil
    {
        private static readonly Dictionary<WeakReference<UIScrollView>, NSTimer> _timers = 
            new Dictionary<WeakReference<UIScrollView>, NSTimer>();

        public static nfloat ActualContentOffset(this UIScrollView scrollView)
        {
            var result = scrollView.ContentOffset.Y + scrollView.ContentInset.Top;
            return result;
        }

        public static void SetActualContentOffset(this UIScrollView scrollView, nfloat height, bool animated)
        {
            scrollView.SetContentOffset(
                new CGPoint(0, height - scrollView.ContentInset.Top), 
                animated);
        }

        public static void ScrollOutHeader(
            UIScrollView scrollView, 
            nfloat headerHeight, 
            bool animated)
        {
            if (scrollView.ContentSize.Height > headerHeight)
            {
                scrollView.SetActualContentOffset(headerHeight, animated);

                if (scrollView.Hidden)
                {
                    scrollView.SetHidden(false, true);
                }
            }
        }

        public static void ScrollOutHeaderAfterReload(
            UIScrollView scrollView, 
            nfloat headerHeight, 
            IListViewSource viewSource,
            bool animated)
        {
            var existingKey = GetTimerKey(scrollView);

            if (viewSource.ItemsSource != null &&
                viewSource.ItemsSource.Cast<object>().Any())
            {
                if (existingKey == null)
                {
                    var newKey = new WeakReference<UIScrollView>(scrollView);

                    _timers[newKey] = 
                        NSTimer.CreateRepeatingScheduledTimer(
                            TimeSpan.MinValue, 
                            t =>
                            {
                                if (scrollView.ContentSize.Height > headerHeight)
                                {
                                    ScrollOutHeader(scrollView, headerHeight, animated);
                                    DisposeTimer(newKey);
                                }
                            });
                }
            }
            else
            {
                DisposeTimer(existingKey);
            }
        }

        public static void AdjustHeaderPosition(UIScrollView scrollView, nfloat headerHeight, bool animation)
        {
            var scrollViewOffset = scrollView.ActualContentOffset();
            if (scrollViewOffset < 0 || scrollView.Decelerating) return;

            if (scrollViewOffset < headerHeight / 2)
            {
                scrollView.SetActualContentOffset(0, animation);
            }
            else if (scrollViewOffset < headerHeight)
            {
                scrollView.SetActualContentOffset(headerHeight, animation);
            }
        }

        private static WeakReference<UIScrollView> GetTimerKey(UIScrollView scrollView)
        {
            var currentKey = default(WeakReference<UIScrollView>);

            var deadRefs = _timers.Keys
                .Where(k =>
                {
                    UIScrollView target;

                    var isDead = !k.TryGetTarget(out target);
                    if (!isDead && target == scrollView)
                    {
                        currentKey = k;
                    }

                    return isDead;
                })
                .ToArray();

            foreach (var deadRef in deadRefs)
            {
                DisposeTimer(deadRef);
            }

            return currentKey;
        }

        private static void DisposeTimer(WeakReference<UIScrollView> key)
        {
            if (key != null)
            {
                _timers[key].Invalidate();
                _timers[key].Dispose();
                _timers.Remove(key);
            }
        }
    }
}