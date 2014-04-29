using System;
using Cirrious.CrossCore;
using Cirrious.CrossCore.Exceptions;
using Cirrious.MvvmCross.Plugins.DownloadCache;
using Cirrious.MvvmCross.Plugins.DownloadCache.Touch;
using MonoTouch.UIKit;
using MvxOriginal = Cirrious.CrossCore.Mvx;

namespace SmartWalk.Client.iOS.Utils.Mvx
{
    /// <summary>
    /// This utility class contains extended functionality of MvvmCross framework.
    /// </summary>
    public static class MvxPlus
    {
        private static MvxDownloadCacheConfiguration DownloadCacheConfig;

        public static IMvxFileDownloadCache SafeGetDownloadCache()
        {
            IMvxFileDownloadCache downloadCache;

            if (MvxOriginal.TryResolve(out downloadCache))
            {
                return downloadCache;
            }

            throw new MvxException("You must call EnsureLoaded on the File plugin before using the DownloadCache");
        }

        public static IMvxFileDownloadCache CreateDownloadCache()
        {
            var configuration = GetDownloadCacheConfig();

            var downloadCache = new MvxFileDownloadCache(
                configuration.CacheName,
                configuration.CacheFolderPath,
                configuration.MaxFiles,
                configuration.MaxFileAge);
            return downloadCache;
        }

        public static IMvxImageCache<UIImage> CreateImageCache()
        {
            var configuration = GetDownloadCacheConfig();

            var fileDownloadCache = MvxOriginal.Resolve<IMvxFileDownloadCache>();
            var fileCache = new MvxImageCache<UIImage>(
                fileDownloadCache, 
                configuration.MaxInMemoryFiles, 
                configuration.MaxInMemoryBytes);
            return fileCache;
        }

        public static IMvxHttpFileDownloader CreateHttpFileDownloader()
        {
            return new MvxFastHttpFileDownloader();
        }

        public static MvxDownloadCacheConfiguration GetDownloadCacheConfig()
        {
            if (DownloadCacheConfig == null)
            {
                DownloadCacheConfig = new MvxDownloadCacheConfiguration {
                    CacheName = "Pictures.MvvmCross",
                    CacheFolderPath = "../Library/Caches/Pictures.MvvmCross/",
                    MaxFiles = 500,
                    MaxFileAge = TimeSpan.FromDays(10),
                    MaxInMemoryBytes = 4000000, // 4 MB
                    MaxInMemoryFiles = 30,
                };
            }
            return DownloadCacheConfig;
        }
    }
}