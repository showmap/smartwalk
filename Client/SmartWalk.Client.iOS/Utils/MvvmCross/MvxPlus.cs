using System;
using Cirrious.CrossCore;
using Cirrious.CrossCore.Exceptions;
using Cirrious.MvvmCross.Plugins.DownloadCache;
using Cirrious.MvvmCross.Plugins.DownloadCache.Touch;
using MonoTouch.UIKit;

namespace SmartWalk.Client.iOS.Utils.MvvmCross
{
    /// <summary>
    /// This utility class contains extended functionality of MvvmCross framework.
    /// </summary>
    public static class MvxPlus
    {
        public static string SizeParam = "#size>";

        public readonly static MvxDownloadCacheConfiguration CacheConfig;
        public readonly static MvxDownloadCacheConfiguration ResizedCacheConfig;

        static MvxPlus()
        {
            CacheConfig = new MvxDownloadCacheConfiguration {
                CacheName = "Pictures.MvvmCross",
                CacheFolderPath = "../Library/Caches/Pictures.MvvmCross/",
                MaxFiles = 500,
                MaxFileAge = TimeSpan.FromDays(10),
                MaxInMemoryBytes = 1000000, // 1 MB
                MaxInMemoryFiles = 10,
            };

            ResizedCacheConfig = new MvxDownloadCacheConfiguration {
                CacheName = "ResizedPictures.MvvmCross",
                CacheFolderPath = "../Library/Caches/ResizedPictures.MvvmCross/",
                MaxFiles = 1000,
                MaxFileAge = TimeSpan.FromDays(10),
                MaxInMemoryBytes = 3000000, // 3 MB
                MaxInMemoryFiles = 30,
            };
        }

        public static IMvxImageCache<UIImage> SafeGetImageCache()
        {
            IMvxImageCache<UIImage> imageCache;

            if (Mvx.TryResolve(out imageCache))
            {
                return imageCache;
            }

            throw new MvxException("You must call EnsureLoaded on the File plugin before using the DownloadCache");
        }

        public static IMvxImageCache<UIImage> CreateImageCache()
        {
            var fileDownloadCache = CreateFileDownloadCache(CacheConfig);
            var fileCache = new MvxImageCache<UIImage>(
                fileDownloadCache, 
                CacheConfig.MaxInMemoryFiles, 
                CacheConfig.MaxInMemoryBytes);
            return fileCache;
        }

        public static IMvxResizedImageCache<UIImage> CreateResizedImageCache()
        {
            var fileDownloadCache = CreateFileDownloadCache(ResizedCacheConfig);
            var fileCache = new MvxResizedImageCache<UIImage>(
                fileDownloadCache, 
                ResizedCacheConfig.MaxInMemoryFiles, 
                ResizedCacheConfig.MaxInMemoryBytes);
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