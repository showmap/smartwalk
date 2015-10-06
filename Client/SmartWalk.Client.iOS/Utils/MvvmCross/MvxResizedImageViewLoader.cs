using System;
using Cirrious.CrossCore;
using Cirrious.CrossCore.Core;
using Cirrious.CrossCore.WeakSubscription;
using Cirrious.MvvmCross.Binding;
using CoreGraphics;
using UIKit;
using ImageState = Cirrious.MvvmCross.Plugins.DownloadCache.MvxDynamicImageHelper<UIKit.UIImage>.ImageState;

namespace SmartWalk.Client.iOS.Utils.MvvmCross
{
    public class MvxResizedImageViewLoader : IDisposable
    {
        private readonly nfloat ScreenScale = UIScreen.MainScreen.Scale;
        private readonly Func<CGRect> _imageFrameAccess;
        private readonly IMvxResizedImageHelper<UIImage> _imageHelper;
        private readonly Action<UIImage, ImageState> _imageSetAction;

        private IDisposable _subscription;

        private string _imageUrl;

        public MvxResizedImageViewLoader(
            Func<UIImageView> imageViewAccess, 
            Func<CGRect> imageFrameAccess,
            Action<ImageState> afterImageChangeAction = null)
        {
            _imageFrameAccess = imageFrameAccess;
            _imageSetAction = (image, state) =>
                {
                    OnImage(imageViewAccess(), image);

                    if (afterImageChangeAction != null)
                    {
                        UIApplication.SharedApplication
                            .InvokeOnMainThread(() => afterImageChangeAction(state));
                    }
                };

            if (!Mvx.TryResolve(out _imageHelper))
            {
                MvxBindingTrace.Error("Can't resolve IMvxResizedImageHelper");
                return;
            }

            var eventInfo = _imageHelper.GetType().GetEvent("ImageChanged");
            _subscription = eventInfo.WeakSubscribe<UIImage>(_imageHelper, ImageHelperOnImageChanged);
        }

        public bool UseRoundClip { get; set; }
        public bool UseGradient { get; set; }

        ~MvxResizedImageViewLoader()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Note - this is public because we use it in weak referenced situations
        public virtual void ImageHelperOnImageChanged(
            object sender, 
            MvxValueEventArgs<UIImage> mvxValueEventArgs)
        {
            var helper = (IMvxResizedImageHelper<UIImage>)sender;
            _imageSetAction(mvxValueEventArgs.Value, helper.CurrentImageState);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_subscription != null)
                {
                    _subscription.Dispose();
                    _subscription = null;
                }
            }
        }

        public string ImageUrl
        {
            get { return _imageUrl; }
            set 
            { 
                if (_imageUrl != value)
                {
                    _imageUrl = value;
                    _imageHelper.ImageUrl = 
                        GetImageUrlWithDimensions(_imageUrl, _imageFrameAccess());
                }
            }
        }

        public string DefaultImagePath
        {
            get { return _imageHelper.DefaultImagePath; }
            set { _imageHelper.DefaultImagePath = value; }
        }

        public string ErrorImagePath
        {
            get { return _imageHelper.ErrorImagePath; }
            set { _imageHelper.ErrorImagePath = value; }
        }

        private static void OnImage(UIImageView imageView, UIImage image)
        {
            if (imageView != null && image != null)
            {
                imageView.Image = image;
            }
        }

        private string GetImageUrlWithDimensions(string imageUrl, CGRect rect)
        {
            var dimensions = !rect.IsEmpty
                ? string.Format(
                    "{0}{1}>{2}{3}{4}",
                    MvxPlus.SizeParam,
                    (int)Math.Ceiling(rect.Width * ScreenScale), 
                    (int)Math.Ceiling(rect.Height * ScreenScale),
                    UseRoundClip ? string.Format(">{0}>{1}", MvxPlus.ClipParam, "true") : string.Empty,
                    UseGradient ? string.Format(">{0}>{1}", MvxPlus.GradientParam, "true") : string.Empty)
                : string.Empty;
            return imageUrl + dimensions;
        }
    }
}