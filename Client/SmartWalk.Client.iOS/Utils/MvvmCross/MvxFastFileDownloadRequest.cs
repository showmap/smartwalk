using System;
using CoreGraphics;
using System.IO;
using System.Threading.Tasks;
using Cirrious.CrossCore.Core;
using Cirrious.MvvmCross.Plugins.DownloadCache;
using UIKit;
using SmartWalk.Client.iOS.Services;

namespace SmartWalk.Client.iOS.Utils.MvvmCross
{
    public class MvxFastFileDownloadRequest : MvxAllThreadDispatchingObject
    {
        public MvxFastFileDownloadRequest(string url, string downloadPath)
        {
            Url = url;
            DownloadPath = downloadPath;
        }

        public string DownloadPath { get; private set; }
        public string Url { get; private set; }

        public event EventHandler<MvxFileDownloadedEventArgs> DownloadComplete;
        public event EventHandler<MvxValueEventArgs<Exception>> DownloadFailed;

        public void Start()
        {
            if (Url.Contains(MvxPlus.SizeParam))
            {
                StartResizedImageRequest();
            }
            else
            {
                StartHttpRequest();
            }
        }

        private void StartHttpRequest()
        {
            var client = HttpService.CreateHttpClient();
            client
                .GetAsync(Url)
                .ContinueWith(
                    response =>
                    {
                        var httpResult = response.Result;
                        httpResult.EnsureSuccessStatusCode(); 
                        httpResult
                            .Content
                            .ReadAsStreamAsync()
                            .ContinueWith(
                                task => HandleSuccess(task.Result), 
                                TaskContinuationOptions.NotOnFaulted)
                            .ContinueWith(
                                ae => FireDownloadFailed(ae.Exception), 
                                TaskContinuationOptions.OnlyOnFaulted);

                    })
                .ContinueWith(
                    ae => FireDownloadFailed(ae.Exception), 
                    TaskContinuationOptions.OnlyOnFaulted);
        }

        private void StartResizedImageRequest()
        {
            var sizeParamIndex = Url.IndexOf(MvxPlus.SizeParam, StringComparison.OrdinalIgnoreCase);
            var originalUrl = Url.Substring(0, sizeParamIndex);
            var sizeParamEndIndex = sizeParamIndex + MvxPlus.SizeParam.Length;
            var sizeParam = Url.Substring(sizeParamEndIndex, Url.Length - sizeParamEndIndex);
            var sizes = sizeParam.Split('>');
            var size = new CGSize(float.Parse(sizes[0]), float.Parse(sizes[1]));

            var imageCache = MvxPlus.SafeGetImageCache();

            imageCache.RequestImage(
                originalUrl, 
                image =>
                RunSyncOrAsyncWithLock(() =>
                {
                    var scaledSize = ResizeToFit(image.Size, size);

                    UIGraphics.BeginImageContextWithOptions(size, true, 1);

                    image.Draw(new CGRect(
                        new CGPoint(
                            -((scaledSize.Width - size.Width) / 2), 
                            -((scaledSize.Height - size.Height) / 2)),
                        scaledSize));

                    var resizedImage = UIGraphics.GetImageFromCurrentImageContext();

                    UIGraphics.EndImageContext();

                    var stream = resizedImage.AsJPEG(0.86f).AsStream();
                    HandleSuccess(stream);
                }),
                FireDownloadFailed);
        }

        private void HandleSuccess(Stream result)
        {
            try
            {
                var fileService = MvxFileStoreHelper.SafeGetFileStore();
                var tempFilePath = DownloadPath + ".tmp";

                using (result)
                {
                    fileService.WriteFile(tempFilePath, result.CopyTo);
                }

                fileService.TryMove(tempFilePath, DownloadPath, true);
            }
            catch (Exception exception)
            {
                FireDownloadFailed(exception);
                return;
            }

            FireDownloadComplete();
        }

        private void FireDownloadFailed(Exception exception)
        {
            var handler = DownloadFailed;
            if (handler != null)
            {
                handler(this, new MvxValueEventArgs<Exception>(exception));
            }
        }

        private void FireDownloadComplete()
        {
            var handler = DownloadComplete;
            if (handler != null)
            {
                handler(this, new MvxFileDownloadedEventArgs(Url, DownloadPath));
            }
        }

        public static CGSize ResizeToFit(CGSize imageSize, CGSize frameSize)
        {
            var widthScale = frameSize.Width / imageSize.Width;
            var heightScale = frameSize.Height / imageSize.Height;
            var scale = (nfloat)Math.Max(widthScale, heightScale);
            return new CGSize(imageSize.Width * scale, imageSize.Height * scale);
        }
    }
}