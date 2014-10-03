using MonoTouch.UIKit;
using System;
using System.Linq;

namespace SmartWalk.Client.iOS.Utils
{
    public class ScrollToHideUIManager
    {
        private readonly UIScrollView _scrollView;
        private readonly double? _refreshControlHeight;

        private double? _startOffsetY;
        private double? _lastOffsetY;
        private bool _isDragging;

        public ScrollToHideUIManager(UIScrollView scrollView)
        {
            _scrollView = scrollView;

            var refreshControl = _scrollView.Subviews
                .OfType<UIRefreshControl>()
                .FirstOrDefault();
            _refreshControlHeight = refreshControl != null
                ? refreshControl.Frame.Height
                : (double?)null;
        }

        public void DraggingStarted()
        {
            _isDragging = true;
            _startOffsetY = _scrollView.ContentOffset.Y;
        }

        public void DraggingEnded()
        {
            _isDragging = false;
            _startOffsetY = null;
            _lastOffsetY = null;
        }

        public void Scrolled()
        {
            // only if it's being dragged by user
            if (_isDragging && !IsCausedByRefreshControl())
            {
                var delta = GetTresholdDelta();
                if (delta == null || delta > 30)
                {
                    _startOffsetY = null; // if show/hide started, forget about delta

                    if (_lastOffsetY > _scrollView.ContentOffset.Y)
                    {
                        OnShowUI();
                    }
                    else if (_lastOffsetY < _scrollView.ContentOffset.Y)
                    {
                        OnHideUI();
                    }
                }

                _lastOffsetY = _scrollView.ContentOffset.Y;
            }
        }

        public void ScrollFinished()
        {
            if (Math.Abs(_scrollView.ContentOffset.Y) < UIConstants.Epsilon)
            {
                OnShowUI();
            }
        }

        public void Reset()
        {
            OnShowUI();
        }

        protected virtual void OnHideUI()
        {
            NavBarManager.Instance.SetHidden(true, true);
        }

        protected virtual void OnShowUI()
        {
            NavBarManager.Instance.SetHidden(false, true);
        }

        // Gets a little treshold delta before show/hide UI is starting
        private double? GetTresholdDelta()
        {
            return _startOffsetY.HasValue
                ? Math.Abs(Math.Abs(_startOffsetY.Value) - Math.Abs(_scrollView.ContentOffset.Y))
                : (double?)null;
        }

        // HACK: skip Scrolled() caused by Pull-To-Refresh when offset.Y == refreshControl.Height
        private bool IsCausedByRefreshControl()
        {
            return _refreshControlHeight.HasValue && 
                Math.Abs(_refreshControlHeight.Value + _scrollView.ContentOffset.Y) < UIConstants.Epsilon;
        }
    }
}