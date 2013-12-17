using System;
using System.Drawing;
using System.Linq;
using Cirrious.MvvmCross.Binding.Touch.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Core.Utils;
using SmartWalk.iOS.Resources;

namespace SmartWalk.iOS.Views.Common
{
    public partial class ImageFullscreenView : UIViewController
    {
        private string _imageUrl;
        private MvxImageViewLoader _imageHelper;
        private UITapGestureRecognizer _singleTapRecognizer;
        private UITapGestureRecognizer _doubleTapRecognizer;
        private UISwipeGestureRecognizer _swipeRecognizer;

        public ImageFullscreenView() : base("ImageFullscreenView", null)
        {
            WantsFullScreenLayout = true;
            ModalTransitionStyle = UIModalTransitionStyle.CrossDissolve;
        }

        public event EventHandler Shown;
        public event EventHandler Hidden;

        public string ImageURL
        {
            get
            {
                return _imageUrl;
            }
            set
            {
                if (_imageUrl != value)
                {
                    _imageUrl = value;

                    if (_imageHelper != null)
                    {
                        _imageHelper.ImageUrl = _imageUrl;
                    }
                }
            }
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            ImageView.ActivityIndicatorViewStyle = UIActivityIndicatorViewStyle.White;

            // HACK: Removing all constraints defined in IB to avoid flickering on zooming
            ScrollView.RemoveConstraints(ScrollView.Constraints);

            ScrollView.ViewForZoomingInScrollView += (UIScrollView sv) => {
                return ImageView;
            };

            _imageHelper = new MvxImageViewLoader(
                () => ImageView,
                () => {
                    if (_imageHelper.ImageUrl != null && 
                        ImageView.Image != null)
                    {
                        ImageView.StopProgress();
                    }
                    else if (_imageHelper.ImageUrl == null)
                    {
                        ImageView.StopProgress();
                    }
                    else
                    {
                        ImageView.StartProgress();
                    }

                    ScrollView.UpdateZoomConstants();
                });

            _imageHelper.ImageUrl = ImageURL;

            InitializeGestures();
            InitializeStyle();
        }

        public void Show()
        {
            if (PresentingViewController == null)
            {
                var window = UIApplication.SharedApplication.Windows[0];
                window.RootViewController.PresentViewController(this, true, null);

                UIApplication.SharedApplication.SetStatusBarHidden(
                    true,
                    UIStatusBarAnimation.Slide);

                if (Shown != null)
                {
                    Shown(this, EventArgs.Empty);
                }
            }
        }

        public void Hide()
        {
            if (PresentingViewController != null)
            {
                UIApplication.SharedApplication.SetStatusBarHidden(
                    false,
                    UIStatusBarAnimation.Slide);

                DismissViewController(true, null);

                if (Hidden != null)
                {
                    Hidden(this, EventArgs.Empty);
                }
            }
        }

        // HACK: to trigger scrollview layoutSubviews on rotation
        public override void WillRotate(UIInterfaceOrientation toInterfaceOrientation, double duration)
        {
            base.WillRotate(toInterfaceOrientation, duration);

            ScrollView.SetNeedsLayout();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            ConsoleUtil.LogDisposed(this);
        }

        partial void OnCloseButtonTouchUpInside(UIButton sender, UIEvent @event)
        {
            Hide();
        }

        private void InitializeGestures()
        {
            _singleTapRecognizer = new UITapGestureRecognizer(
                () => CloseButton.Hidden = !CloseButton.Hidden) {
                NumberOfTapsRequired = (uint)1
            };

            _doubleTapRecognizer = new UITapGestureRecognizer(ZoomTo) {
                NumberOfTapsRequired = (uint)2
            };

            _swipeRecognizer = new UISwipeGestureRecognizer(Hide) {
                Direction = UISwipeGestureRecognizerDirection.Down
            };

            _singleTapRecognizer.RequireGestureRecognizerToFail(_doubleTapRecognizer);

            ScrollView.AddGestureRecognizer(_singleTapRecognizer);
            ScrollView.AddGestureRecognizer(_doubleTapRecognizer);
            ScrollView.AddGestureRecognizer(_swipeRecognizer);
        }

        private void InitializeStyle()
        {
            CloseButton.SetImage(ThemeIcons.CloseWhite, UIControlState.Normal);
        }

        private void ZoomTo()
        {
            if (Math.Abs(ScrollView.MinimumZoomScale - ScrollView.MaximumZoomScale) < 0.1f)
            {
                return;
            }

            if (ScrollView.ZoomScale > ScrollView.MinimumZoomScale)
            {
                ScrollView.SetZoomScale(ScrollView.MinimumZoomScale, true);
            }
            else
            {
                var point = _doubleTapRecognizer.LocationInView(ScrollView);
                var zoomRect = GetZoomRect(ScrollView.MaximumZoomScale, point);
                ScrollView.ZoomToRect(zoomRect, true);
            }
        }

        private RectangleF GetZoomRect(float scale, PointF center)
        {
            var size = new SizeF(
                ImageView.Frame.Size.Height / scale,
                ImageView.Frame.Size.Width / scale);

            var centerInImageView = ScrollView.ConvertPointToView(center, ImageView);
            var point = new PointF(
                centerInImageView.X - (size.Width / 2.0f),
                centerInImageView.Y - (size.Height / 2.0f));

            var result = new RectangleF(point, size);
            return result;
        }
    }

    [Register("ImageZoomScrollView")]
    public class ImageZoomScrollView : UIScrollView
    {
        private SizeF _lastBoundsSize;

        public ImageZoomScrollView(IntPtr handle) : base(handle)
        {
        }

        public UIImageView ImageView
        {
            get { return Subviews.OfType<UIImageView>().FirstOrDefault(); }
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            if (_lastBoundsSize != Bounds.Size)
            {
                UpdateZoomConstants();
                // to avoid ios exception
                base.LayoutSubviews();

                _lastBoundsSize = Bounds.Size;
            }

            ImageView.Frame = GetCenteredImageFrame();
        }

        public void UpdateZoomConstants()
        {
            var imageSize = ImageView.Image != null
                ? ImageView.Image.Size
                : Bounds.Size;

            var widthScale = Bounds.Size.Width / imageSize.Width;
            var heightScale = Bounds.Size.Height / imageSize.Height;

            MinimumZoomScale = 
            Math.Min(MaximumZoomScale, Math.Min(widthScale, heightScale));
            SetZoomScale(MinimumZoomScale, false);
            SetContentOffset(PointF.Empty, false);
        }

        private RectangleF GetCenteredImageFrame()
        {
            var imageFrame = ImageView.Image != null 
                ? ImageView.Frame
                : Bounds;
            var result = imageFrame;

            if (imageFrame.Width < Bounds.Size.Width)
            {
                result.X = (Bounds.Size.Width - imageFrame.Width) / 2;
            }
            else
            {
                result.X = 0;
            }

            if (imageFrame.Height < Bounds.Size.Height)
            {
                result.Y = (Bounds.Size.Height - imageFrame.Height) / 2;
            }
            else
            {
                result.Y = 0;
            }

            return result;
        }
    }
}