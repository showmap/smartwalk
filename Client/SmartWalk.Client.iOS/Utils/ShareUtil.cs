using UIKit;
using Foundation;

namespace SmartWalk.Client.iOS.Utils
{
    public static class ShareUtil
    {
        private static NSMutableCharacterSet _urlAllowedCharacterSet;

        static ShareUtil()
        {
            _urlAllowedCharacterSet = new NSMutableCharacterSet();
            _urlAllowedCharacterSet.AddCharacters(new NSString("#"));
            _urlAllowedCharacterSet.UnionWith(NSUrlUtilities_NSCharacterSet.UrlHostAllowedCharacterSet);
            _urlAllowedCharacterSet.UnionWith(NSUrlUtilities_NSCharacterSet.UrlPathAllowedCharacterSet);
            _urlAllowedCharacterSet.UnionWith(NSUrlUtilities_NSCharacterSet.UrlQueryAllowedCharacterSet);
            _urlAllowedCharacterSet.UnionWith(NSUrlUtilities_NSCharacterSet.UrlFragmentAllowedCharacterSet);
        }

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
                .CreateStringByAddingPercentEncoding(_urlAllowedCharacterSet);
            var result = new NSUrl(str);
            return result;
        }
    }
}