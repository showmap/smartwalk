using System;
using System.Drawing;
using System.Linq;
using Cirrious.MvvmCross.Binding.Touch.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.iOS.Utils;

namespace SmartWalk.iOS.Views.Common
{
    public partial class ImageFullscreenController : UIViewController
    {
        private string _imageUrl;
        private MvxImageViewLoader _imageHelper;
        private UITapGestureRecognizer _singleTapRecognizer;
        private UITapGestureRecognizer _doubleTapRecognizer;
        private UISwipeGestureRecognizer _swipeRecognizer;

        public ImageFullscreenController() : base("ImageFullscreenController", null)
        {
            WantsFullScreenLayout = true;
            ModalTransitionStyle = UIModalTransitionStyle.CrossDissolve;
        }

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

            // HACK: Removing all constraints defined in IB to avoid flickering on zooming
            ScrollView.RemoveConstraints(ScrollView.Constraints);

            ScrollView.ViewForZoomingInScrollView += (UIScrollView sv) => {
                return ImageView;
            };

            _imageHelper = new MvxImageViewLoader(
                () => ImageView,
                () => {
                    SetZoomConstants();

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
            });

            _imageHelper.ImageUrl = ImageURL;

            _singleTapRecognizer = new UITapGestureRecognizer(() =>
                    CloseButton.Hidden = !CloseButton.Hidden) {
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

        public override void WillRotate(UIInterfaceOrientation toInterfaceOrientation, double duration)
        {
            base.WillRotate(toInterfaceOrientation, duration);

            ScrollView.SetNeedsLayout();
        }

        public override void DidRotate(UIInterfaceOrientation fromInterfaceOrientation)
        {
            base.DidRotate(fromInterfaceOrientation);

            SetZoomConstants(true);
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
            }

            NSTimer.CreateScheduledTimer(
                TimeSpan.FromSeconds(0.5), 
                new NSAction(() => {
                    ImageURL = null;
                    ImageView.Image = null;
                }));
        }

        partial void OnCloseButtonTouchUpInside(UIButton sender, UIEvent @event)
        {
            Hide();
        }

        private void SetZoomConstants(bool animated = false)
        {
            if (ImageView.Image == null) return;

            var widthScale = View.Bounds.Size.Width / ImageView.Image.Size.Width;
            var heightScale = View.Bounds.Size.Height / ImageView.Image.Size.Height;

            ScrollView.MinimumZoomScale = 
                Math.Min(ScrollView.MaximumZoomScale, Math.Min(widthScale, heightScale));
            ScrollView.SetZoomScale(ScrollView.MinimumZoomScale, animated);
            ScrollView.SetContentOffset(PointF.Empty, animated);
        }

        private void ZoomTo()
        {
            if (ScrollView.MinimumZoomScale == ScrollView.MaximumZoomScale)
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
            ImageView.Frame = GetCenteredImageFrame();
        }

        public RectangleF GetCenteredImageFrame()
        {
            var frame = ImageView.Frame;

            if (ImageView.Frame.Width < Bounds.Size.Width)
            {
                frame.X = (Bounds.Size.Width - ImageView.Frame.Width) / 2;
            }
            else
            {
                frame.X = 0;
            }

            if (ImageView.Frame.Height < Bounds.Size.Height)
            {
                frame.Y = (Bounds.Size.Height - ImageView.Frame.Height) / 2;
            }
            else
            {
                frame.Y = 0;
            }

            return frame;
        }
    }
}