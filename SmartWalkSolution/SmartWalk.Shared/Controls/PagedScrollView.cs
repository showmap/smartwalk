using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace SmartWalk.iOS.Controls
{
    [Register("PagedScrollView")]
    public class PagedScrollView : UIView
    {
        private const int GotoButtonWidth = 60;
        private const int PagerHeight = 27;

        private UIScrollView _scrollView;
        private UIView[] _pageViews;
        private List<UIButton> _gotoButtons;
        private UIPageControl _pageControl;

        private bool _isTouching;

        public PagedScrollView(IntPtr handle) : base(handle)
        {
            Initialize();
        }

        public UIView[] PageViews 
        { 
            get { return _pageViews; }
            set 
            { 
                _pageViews = value;
                Populate();
                SetNeedsLayout();
            } 
        }

        public int CurrentPage
        {
            get
            {
                return _pageControl.CurrentPage;
            }
            set
            {
                _pageControl.CurrentPage = value;
                ScrollToCurrentPage(false);
            }
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            var pagerHeight = _pageControl.Pages < 2 ? 0 : PagerHeight;

            // SCROLL

            var scrollSize = Frame.Size;
            scrollSize.Height = Frame.Size.Height - pagerHeight;

            _scrollView.Frame = new RectangleF(PointF.Empty, scrollSize);

            _scrollView.ContentSize = new SizeF(
                _scrollView.Frame.Width * _pageControl.Pages,
                _scrollView.Frame.Height);

            for (var i = 0; i < _pageControl.Pages; i++)
            {
                PageViews[i].Frame = new RectangleF(
                    new PointF(i * _scrollView.Frame.Width, 0),
                    _scrollView.Frame.Size);
            }


            // BUTTONS

            for (var i = 0; i < _pageControl.Pages - 1; i++)
            {
                _gotoButtons[i].Frame = new RectangleF(
                    PageViews[i + 1].Frame.X - GotoButtonWidth / 2,
                    PageViews[i].Frame.Y,
                    GotoButtonWidth,
                    PageViews[i].Frame.Height);
            }


            // PAGER

            _pageControl.Frame = new RectangleF(
                (Frame.Size.Width - _pageControl.IntrinsicContentSize.Width) / 2, 
                Frame.Size.Height - pagerHeight, 
                _pageControl.IntrinsicContentSize.Width, 
                pagerHeight);

            ScrollToCurrentPage(false);
        }

        [Export("scrollViewDidScroll:")]
        public void Scrolled(UIScrollView scrollView)
        {
            if (_isTouching)
            {
                var pageWidth = _scrollView.Frame.Size.Width;
                var page = Math.Floor((_scrollView.ContentOffset.X - pageWidth / 2) / pageWidth) + 1;
                _pageControl.CurrentPage = (int)page;
            }
        }

        [Export("scrollViewWillBeginDragging:")]
        public void DraggingStarted(UIScrollView scrollView)
        {
            _isTouching = true;
        }

        [Export("scrollViewDidEndDecelerating:")]
        public void DecelerationEnded(UIScrollView scrollView)
        {
            _isTouching = false;
        }

        private void Initialize()
        {
            // SCROLL

            _scrollView = new UIScrollView();
            _scrollView.ShowsHorizontalScrollIndicator = false;
            _scrollView.ShowsVerticalScrollIndicator = false;
            _scrollView.PagingEnabled = true;
            _scrollView.WeakDelegate = this;

            AddSubviews(_scrollView);


            // BUTTONS

            _gotoButtons = new List<UIButton>();


            // PAGER

            _pageControl = new UIPageControl();
            _pageControl.HidesForSinglePage = true;
            _pageControl.CurrentPageIndicatorTintColor = UIColor.FromRGB(0, 128, 255);
            _pageControl.PageIndicatorTintColor = UIColor.LightGray;
            _pageControl.ValueChanged += OnPageControlValueChanged;

            AddSubviews(_pageControl);
        }

        private void Populate()
        {
            if (_scrollView.Subviews != null)
            {
                foreach (var view in _scrollView.Subviews.ToArray())
                {
                    view.RemoveFromSuperview();
                }
            }

            // SCROLL

            if (PageViews != null)
            {
                _scrollView.AddSubviews(PageViews);
            }

            // BUTTONS

            if (PageViews != null)
            {
                for (var i = 0; i < PageViews.Length - 1; i++)
                {
                    var button = new UIButton(UIButtonType.Custom) { Tag = i };
                    button.TouchUpInside += OnGotoButtonTouchUpInside;
                    _gotoButtons.Add(button);
                }

                _scrollView.AddSubviews(_gotoButtons.ToArray());
            }

            // PAGER

            _pageControl.Pages = PageViews != null ? PageViews.Length : 0;
            _pageControl.CurrentPage = 0;
        }

        private void OnGotoButtonTouchUpInside(object sender, EventArgs e)
        {
            var button = (UIButton)sender;

            _pageControl.CurrentPage = button.Tag + 
                (_pageControl.CurrentPage == button.Tag ? 1 : 0);

            ScrollToCurrentPage(true);
        }

        private void OnPageControlValueChanged(object sender, EventArgs e)
        {
            ScrollToCurrentPage(true);
        }

        private void ScrollToCurrentPage(bool animated)
        {
            var frame = new RectangleF
            {
                X = _scrollView.Frame.Size.Width * _pageControl.CurrentPage,
                Y = 0,
                Size = _scrollView.Frame.Size
            };

            _scrollView.ScrollRectToVisible(frame, animated);
        }
    }
}

