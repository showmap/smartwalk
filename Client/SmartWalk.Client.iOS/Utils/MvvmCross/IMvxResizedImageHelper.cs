using Cirrious.CrossCore.Platform;

namespace SmartWalk.Client.iOS.Utils.MvvmCross
{
    public interface IMvxResizedImageHelper<T> : IMvxImageHelper<T> where T : class
    {
        ImageState CurrentImageState { get; }
    }
}