using System;
using System.Drawing;
using Cirrious.CrossCore.Core;
using Cirrious.MvvmCross.Binding;
using Cirrious.MvvmCross.Binding.Views;
using Cirrious.MvvmCross.Plugins.DownloadCache;
using MonoTouch.UIKit;

namespace SmartWalk.Client.iOS.Utils.Mvx
{
    public class MvxResizedImageViewLoader
        : MvxBaseImageViewLoader<UIImage>
    {
        private readonly Func<UIImageView> _imageViewAccess;

        private string _imageUrl;

        public MvxResizedImageViewLoader(
            Func<UIImageView> imageViewAccess, 
            Action afterImageChangeAction = null)
                : base(
                image =>
                {
                    OnImage(imageViewAccess(), image);

                    if (afterImageChangeAction != null)
                    {
                        afterImageChangeAction();
                    }
                })
        {
            _imageViewAccess = imageViewAccess;
        }

        public new string ImageUrl
        {
            get { return _imageUrl; }
            set
            { 
                _imageUrl = value;

                var imageUrlWithDimens = 
                    GetImageUrlWithDimensions(_imageUrl, _imageViewAccess());
                base.ImageUrl = imageUrlWithDimens;
            }
        }

        public override void ImageHelperOnImageChanged(
            object sender, 
            MvxValueEventArgs<UIImage> mvxValueEventArgs)
        {
            base.ImageHelperOnImageChanged(sender, mvxValueEventArgs);

            var imageView = _imageViewAccess();
            var image = mvxValueEventArgs.Value;

            if (imageView != null && 
                image != null &&
                image.Size != new SizeF(
                    imageView.Bounds.Size.Width * UIScreen.MainScreen.Scale,
                    imageView.Bounds.Size.Height * UIScreen.MainScreen.Scale))
            {
                SaveResizedImage(imageView);
            }
        }

        private static void OnImage(UIImageView imageView, UIImage image)
        {
            if (imageView != null && image != null)
            {
                imageView.Image = image;
            }
        }

        private void SaveResizedImage(UIView imageView)
        {
            var downloadCache = MvxPlus.SafeGetDownloadCache();
            if (downloadCache == null) return;

            var resizedImage = GetResizedImage(imageView);
            
            downloadCache.RequestLocalFilePath(
                base.ImageUrl,
                imagePath => 
                {
                    try
                    {
                        var fileService = MvxFileStoreHelper.SafeGetFileStore();
                        var tempFilePath = imagePath + ".tmp";

                        using (var stream = resizedImage.AsJPEG(0.9f).AsStream())
                        {
                            fileService.WriteFile(tempFilePath, stream.CopyTo);
                        }

                        fileService.TryMove(tempFilePath, imagePath, true);
                    }
                    catch (Exception ex)
                    {
                        MvxBindingTrace.Error("Unable to save resized image: {0}", ex.Message);
                    }
                },
                ex => MvxBindingTrace.Error("Unable to save resized image: {0}", ex.Message));
        }

        private static UIImage GetResizedImage(UIView imageView)
        {
            UIGraphics.BeginImageContextWithOptions(
                imageView.Bounds.Size, 
                true, 
                UIScreen.MainScreen.Scale);

            if (UIDevice.CurrentDevice.CheckSystemVersion(7, 0))
            {
                imageView.DrawViewHierarchy(imageView.Bounds, true);
            }
            else
            {
                imageView.Layer.RenderInContext(UIGraphics.GetCurrentContext());
            }

            var result = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();

            return result;
        }

        private static string GetImageUrlWithDimensions(string imageUrl, UIView view)
        {
            var dimensions = view != null && !view.Frame.IsEmpty
                ? string.Format("#({0},{1})", view.Frame.Width, view.Frame.Height)
                : string.Empty;
            return imageUrl + dimensions;
        }
    }
}