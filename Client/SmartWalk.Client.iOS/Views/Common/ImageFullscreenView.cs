using System;
using CoreGraphics;
using System.Linq;
using Cirrious.MvvmCross.Binding.Touch.Views;
using Foundation;
using UIKit;
using SmartWalk.Shared.Utils;
using SmartWalk.Client.iOS.Resources;
using SmartWalk.Client.iOS.Utils;
using SmartWalk.Client.iOS.Controls;

namespace SmartWalk.Client.iOS.Views.Common
{
    public partial class ImageFullscreenView : UIViewController, IFullscreenView
    {
        private string _imageUrl;
        private MvxImageViewLoader _imageHelper;
        private UITapGestureRecognizer _singleTapRecognizer;
        private UITapGestureRecognizer _doubleTapRecognizer;
        private UISwipeGestureRecognizer _swipeRecognizer;

        public ImageFullscreenView() : base("ImageFullscreenView", null)
        {
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
            ScrollView.ViewForZoomingInScrollView += sv => ImageView;

            ImageView.StartProgress();
            _imageHelper = new MvxImageViewLoader(
                () => ImageView,
                () => {
                    ScrollView.UpdateZoomConstants();
                    ScrollView.SetNeedsLayout();
                });
            _imageHelper.DefaultImagePath = Theme.DefaultImagePath;
            _imageHelper.ErrorImagePath = Theme.ErrorImagePath;
            _imageHelper.ImageUrl = ImageURL;

            InitializeGestures();
            InitializeStyle();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            UpdateViewConstraints();
            CloseButton.UpdateState();
        }

        // HACK: to trigger scrollview layoutSubviews on rotation
        public override void WillRotate(UIInterfaceOrientation toInterfaceOrientation, double duration)
        {
            base.WillRotate(toInterfaceOrientation, duration);

            ScrollView.SetNeedsLayout();
        }

        public override void WillAnimateRotation(UIInterfaceOrientation toInterfaceOrientation, double duration)
        {
            base.WillAnimateRotation(toInterfaceOrientation, duration);

            UpdateViewConstraints();
            CloseButton.UpdateState();
        }

        public override bool PrefersStatusBarHidden()
        {
            return true;
        }

        public void Show()
        {
            if (PresentingViewController == null)
            {
                UIApplication.SharedApplication.KeyWindow
                    .RootViewController
                    .PresentViewController(this, true, null);

                NavBarManager.Instance.SetHidden(true, true);

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
                DisposeGestures();
                DismissViewController(true, null);

                if (Hidden != null)
                {
                    Hidden(this, EventArgs.Empty);
                }
            }
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

        private new void UpdateViewConstraints()
        {
            var size = ScreenUtil.IsVerticalOrientation 
                ? ButtonBarButton.DefaultVerticalSize
                : ButtonBarButton.DefaultLandscapeSize;
            CloseButtonWidthConstraint.Constant = size.Width;
            CloseButtonHeightConstraint.Constant = size.Height;
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
                Direction = UISwipeGestureRecognizerDirection.Down | 
                    UISwipeGestureRecognizerDirection.Up
            };

            _singleTapRecognizer.RequireGestureRecognizerToFail(_doubleTapRecognizer);

            ScrollView.AddGestureRecognizer(_singleTapRecognizer);
            ScrollView.AddGestureRecognizer(_doubleTapRecognizer);
            ScrollView.AddGestureRecognizer(_swipeRecognizer);
        }

        private void DisposeGestures()
        {
            if (_singleTapRecognizer != null)
            {
                ScrollView.RemoveGestureRecognizer(_singleTapRecognizer);
                _singleTapRecognizer.Dispose();
                _singleTapRecognizer = null;
            }

            if (_doubleTapRecognizer != null)
            {
                ScrollView.RemoveGestureRecognizer(_doubleTapRecognizer);
                _doubleTapRecognizer.Dispose();
                _doubleTapRecognizer = null;
            }

            if (_swipeRecognizer != null)
            {
                ScrollView.RemoveGestureRecognizer(_swipeRecognizer);
                _swipeRecognizer.Dispose();
                _swipeRecognizer = null;
            }
        }

        private void InitializeStyle()
        {
            CloseButton.SemiTransparentType = SemiTransparentType.Dark;
            CloseButton.VerticalIcon = ThemeIcons.Close;
            CloseButton.LandscapeIcon = ThemeIcons.CloseLandscape;
            CloseButton.UpdateState();
        }

        private void ZoomTo()
        {
            if (ScrollView.MinimumZoomScale.EqualsNF(ScrollView.MaximumZoomScale))
            {
                return;
            }

            // HACK: Using custom zoom animation due to jerking in iOS7
            if (ScrollView.ZoomScale > ScrollView.MinimumZoomScale)
            {
                UIView.Animate(
                    UIConstants.AnimationLongerDuration,
                    () =>
                    {
                        ScrollView.SetZoomScale(ScrollView.MinimumZoomScale, false);
                        ScrollView.LayoutSubviews();
                    });
            }
            else
            {
                var center = _doubleTapRecognizer.LocationInView(ImageView);
                var zoomRect = GetZoomRect(ScrollView.MaximumZoomScale, center);

                UIView.Animate(
                    UIConstants.AnimationLongerDuration,
                    () =>
                    {
                        ScrollView.ZoomToRect(zoomRect, false);
                        ScrollView.LayoutSubviews();
                    });
            }
        }

        private CGRect GetZoomRect(nfloat scale, CGPoint center)
        {
            var size = new CGSize(
                ScrollView.Frame.Size.Width / scale,
                ScrollView.Frame.Size.Height / scale);

            var location = new CGPoint(
                center.X - (size.Width / 2.0f),
                center.Y - (size.Height / 2.0f));

            var result = new CGRect(location, size);
            return result;
        }
    }

    [Register("ImageZoomScrollView")]
    public class ImageZoomScrollView : UIScrollView
    {
        private CGSize _lastBoundsSize;

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
            var imageSize = ImageView.HasImage()
                ? ImageView.Image.Size
                : Bounds.Size;

            var widthScale = Bounds.Size.Width / imageSize.Width;
            var heightScale = Bounds.Size.Height / imageSize.Height;

            MinimumZoomScale = 
                (nfloat)Math.Min(MaximumZoomScale, Math.Min(widthScale, heightScale));
            SetZoomScale(MinimumZoomScale, false);
            SetContentOffset(CGPoint.Empty, false);
        }

        private CGRect GetCenteredImageFrame()
        {
            var imageFrame = ImageView.HasImage()
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