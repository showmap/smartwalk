using Cirrious.MvvmCross.Plugins.DownloadCache;

namespace SmartWalk.Client.iOS.Utils.MvvmCross
{
    public class MvxResizedImageCache<T> : MvxImageCache<T>, IMvxResizedImageCache<T>
    {
        public MvxResizedImageCache(
            IMvxFileDownloadCache fileDownloadCache, 
            int maxInMemoryFiles, 
            int maxInMemoryBytes) 
            : base(fileDownloadCache, maxInMemoryFiles, maxInMemoryBytes)
        {
        }
    }
}