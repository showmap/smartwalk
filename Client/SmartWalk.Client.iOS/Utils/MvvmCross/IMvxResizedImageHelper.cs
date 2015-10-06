using Cirrious.CrossCore.Platform;
using ImageState = Cirrious.MvvmCross.Plugins.DownloadCache.MvxDynamicImageHelper<UIKit.UIImage>.ImageState;

namespace SmartWalk.Client.iOS.Utils.MvvmCross
{
    public interface IMvxResizedImageHelper<T> : IMvxImageHelper<T> where T : class
    {
        ImageState CurrentImageState { get; }
    }
}