using MonoTouch.UIKit;

namespace SmartWalk.Client.iOS.Utils
{
    public class ScrollDownToHideUIManager
    {
        private readonly UIScrollView _scrollView;

        private double? _lastOffsetY;
        private bool _isDragging;

        public ScrollDownToHideUIManager(UIScrollView scrollView)
        {
            _scrollView = scrollView;
        }

        public void DraggingStarted()
        {
            _isDragging = true;
        }

        public void DraggingEnded()
        {
            _isDragging = false;
            _lastOffsetY = null;
        }

        public void Scrolled()
        {
            if (_isDragging)
            {
                if (_lastOffsetY > _scrollView.ContentOffset.Y)
                {
                    OnShowUI();
                }
                else if (_lastOffsetY < _scrollView.ContentOffset.Y)
                {
                    OnHideUI();
                }

                _lastOffsetY = _scrollView.ContentOffset.Y;
            }
        }

        public void ScrollFinished()
        {
            if (System.Math.Abs(_scrollView.ContentOffset.Y) < 0.0001)
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
            NavBarManager.Instance.SetNavBarHidden(true, true, true);
        }

        protected virtual void OnShowUI()
        {
            NavBarManager.Instance.SetNavBarHidden(true, false, true);
        }
    }
}