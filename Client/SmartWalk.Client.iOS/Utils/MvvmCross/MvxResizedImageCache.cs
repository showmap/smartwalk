using Cirrious.MvvmCross.Plugins.DownloadCache;

namespace SmartWalk.Client.iOS.Utils.MvvmCross
{
    public class MvxResizedImageCache<T> : MvxImageCache<T>, IMvxResizedImageCache<T>
    {
        public MvxResizedImageCache(
            IMvxFileDownloadCache fileDownloadCache, 
            int maxInMemoryFiles, 
            int maxInMemoryBytes,
            bool disposeOnRemove) 
            : base(fileDownloadCache, maxInMemoryFiles, maxInMemoryBytes, disposeOnRemove)
        {
        }
    }
}