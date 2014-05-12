using MonoTouch.UIKit;
using MonoTouch.Foundation;
using SmartWalk.Client.Core.Services;

namespace SmartWalk.Client.iOS.Services
{
    public class OpenURLTask : IOpenURLTask
    {
        public void OpenURL(string url)
        {
            using (var nsURL = new NSUrl(url))
            {
                if (UIApplication.SharedApplication.CanOpenUrl(nsURL))
                {
                    UIApplication.SharedApplication.OpenUrl(nsURL);
                }
            }
        }
    }
}