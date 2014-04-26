using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace SmartWalk.Client.iOS.Utils
{
    public static class ShareUtil
    {
        public static void Share(UIViewController controller, string text)
        {
            var textToShare = new NSString(text);
            var activityItems = new NSObject[] {textToShare};
            var activityViewController = new UIActivityViewController(activityItems, null);
            controller.PresentViewController(activityViewController, true, null);
        }
    }
}