using System;
using Cirrious.CrossCore;
using Cirrious.CrossCore.Core;

namespace SmartWalk.Client.iOS.Utils.MvvmCross
{
    public class MvxResizedImageRequest<T>
    {
        private readonly string _url;

        public MvxResizedImageRequest(string url)
        {
            _url = url;
        }

        public string Url
        {
            get { return _url; }
        }

        public event EventHandler<MvxValueEventArgs<Exception>> Error;
        public event EventHandler<MvxValueEventArgs<T>> Complete;

        public void Start()
        {
            var cache = Mvx.Resolve<IMvxResizedImageCache<T>>();
            var weakThis = new WeakReference(this);

            cache.RequestImage(
                _url,
                image =>
                {
                    var strongThis = (MvxResizedImageRequest<T>)weakThis.Target;
                    if (strongThis == null)
                        return;

                    var handler = strongThis.Complete;
                    if (handler != null)
                        handler(this, new MvxValueEventArgs<T>(image));
                },
                exception =>
                {
                    var strongThis = (MvxResizedImageRequest<T>)weakThis.Target;
                    if (strongThis == null)
                        return;

                    var handler = strongThis.Error;
                    if (handler != null)
                        handler(this, new MvxValueEventArgs<Exception>(exception));
                });
        }
    }
}