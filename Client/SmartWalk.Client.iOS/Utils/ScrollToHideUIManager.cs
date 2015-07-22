using System;
using System.Linq;
using SmartWalk.Shared.Utils;
using UIKit;

namespace SmartWalk.Client.iOS.Utils
{
    public class ScrollToHideUIManager
    {
        private readonly UIScrollView _scrollView;
        private readonly double? _refreshControlHeight;
        private readonly double _topGap;

        private double? _startOffsetY;
        private double? _lastOffsetY;
        private int _lastActionDirection;
        private bool _isDragging;

        private nfloat OffsetY
        {
            get { return _scrollView.ContentOffset.Y; }
        }

        private bool AtTop
        {
            get { return Math.Abs(_scrollView.ActualContentOffset() - _topGap) < 2; }
        }

        public ScrollToHideUIManager(UIScrollView scrollView, double topGap = 0)
        {
            _scrollView = scrollView;
            _topGap = topGap;

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
            _startOffsetY = OffsetY;
        }

        public void DraggingEnded()
        {
            _isDragging = false;
            _startOffsetY = null;
            _lastOffsetY = null;
            _lastActionDirection = 0;
        }

        public void Scrolled()
        {
            // only if it's being dragged by user
            if (_isDragging && !IsCausedByRefreshControl())
            {
                var direction = _lastOffsetY.HasValue 
                    ? Math.Sign(_lastOffsetY.Value - OffsetY)
                    : 0;

                var delta = _startOffsetY.HasValue 
                    ? _startOffsetY.Value - OffsetY 
                    : (double?)null;
                
                if (direction > 0 ? delta > 50 : delta < -50)
                {
                    _startOffsetY = OffsetY;

                    if (direction != _lastActionDirection)
                    {
                        _lastActionDirection = direction;

                        if (_lastOffsetY > OffsetY)
                        {
                            OnShowUI();
                        }
                        else if (_lastOffsetY < OffsetY)
                        {
                            OnHideUI();
                        }
                    }
                }

                _lastOffsetY = OffsetY;
            }
        }

        public void ScrolledToTop()
        {
            OnShowUI();
        }

        public void ScrollFinished()
        {
            if (AtTop)
            {
                OnShowUI();
            }
        }

        public void Reset()
        {
            DraggingEnded();
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

        // HACK: skip Scrolled() caused by Pull-To-Refresh when offset.Y == refreshControl.Height
        private bool IsCausedByRefreshControl()
        {
            return _refreshControlHeight.HasValue && 
                _refreshControlHeight.Value.EqualsF(Math.Abs(OffsetY));
        }
    }
}