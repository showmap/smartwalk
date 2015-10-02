using Cirrious.CrossCore;
using Cirrious.CrossCore.Exceptions;
using Cirrious.MvvmCross.Plugins.DownloadCache;
using Cirrious.MvvmCross.Plugins.DownloadCache.Touch;
using UIKit;

namespace SmartWalk.Client.iOS.Utils.MvvmCross
{
    /// <summary>
    /// This utility class contains extended functionality of MvvmCross framework.
    /// </summary>
    public static class MvxPlus
    {
        public static string SizeParam = "#size>";
        public static string ClipParam = "clip";
        public static string GradientParam = "gradient";

        public static IMvxImageCache<UIImage> SafeGetImageCache()
        {
            IMvxImageCache<UIImage> imageCache;

            if (Mvx.TryResolve(out imageCache))
            {
                return imageCache;
            }

            throw new MvxException("You must call EnsureLoaded on the File plugin before using the DownloadCache");
        }

        public static IMvxImageCache<UIImage> CreateImageCache(
            MvxDownloadCacheConfiguration config)
        {
            var fileDownloadCache = CreateFileDownloadCache(config);
            var fileCache = new MvxImageCache<UIImage>(
                fileDownloadCache, 
                config.MaxInMemoryFiles, 
                config.MaxInMemoryBytes,
                false);
            return fileCache;
        }

        public static IMvxResizedImageCache<UIImage> CreateResizedImageCache(
            MvxDownloadCacheConfiguration config)
        {
            var fileDownloadCache = CreateFileDownloadCache(config);
            var fileCache = new MvxResizedImageCache<UIImage>(
                fileDownloadCache, 
                config.MaxInMemoryFiles, 
                config.MaxInMemoryBytes,
                false);
            return fileCache;
        }

        public static IMvxHttpFileDownloader CreateHttpFileDownloader()
        {
            return new MvxFastHttpFileDownloader();
        }

        private static IMvxFileDownloadCache CreateFileDownloadCache(
            MvxDownloadCacheConfiguration config)
        {
            var downloadCache = new MvxFileDownloadCache(
                config.CacheName,
                config.CacheFolderPath,
                config.MaxFiles,
                config.MaxFileAge);
            return downloadCache;
        }
    }
}