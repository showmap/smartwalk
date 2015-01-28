using UIKit;
using Foundation;

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

        public static NSUrl ToNSUrl(this string url)
        {
            var str = new NSString(url)
                .CreateStringByAddingPercentEncoding(
                    NSUrlUtilities_NSCharacterSet.UrlQueryAllowedCharacterSet);
            var result = new NSUrl(str);
            return result;
        }
    }
}