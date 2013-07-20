using System;
using MonoTouch.UIKit;
using System.Drawing;

namespace SmartWalk.iOS.Views.Common.EntityCell
{
    public class EntityScrollViewDelegate : UIScrollViewDelegate
    {
        private readonly UIScrollView _scrollView;
        private readonly UIPageControl _pageControl;

        private bool _isPageControlBeingUsed;

        public EntityScrollViewDelegate(UIScrollView scrollView, UIPageControl pageControl)
        {
            _scrollView = scrollView;
            _pageControl = pageControl;
            _pageControl.ValueChanged += OnPageControlValueChanged;
        }

        public override void Scrolled(UIScrollView scrollView)
        {
            if (!_isPageControlBeingUsed)
            {
                var pageWidth = scrollView.Frame.Size.Width;
                var page = Math.Floor((scrollView.ContentOffset.X - pageWidth / 2) / pageWidth) + 1;
                _pageControl.CurrentPage = (int)page;
            }
        }

        public override void DraggingStarted(UIScrollView scrollView)
        {
            _isPageControlBeingUsed = false;
        }

        public override void DraggingEnded(UIScrollView scrollView, bool willDecelerate)
        {
            _isPageControlBeingUsed = false;
        }

        public void ScrollToCurrentPage()
        {
            var frame = new RectangleF
            {
                X = _scrollView.Frame.Size.Width * _pageControl.CurrentPage,
                Y = 0,
                Size = _scrollView.Frame.Size
            };

            _scrollView.ScrollRectToVisible(frame, true);

            _isPageControlBeingUsed = true;
        }

        private void OnPageControlValueChanged(object sender, EventArgs e)
        {
            ScrollToCurrentPage();
        }
    }
}