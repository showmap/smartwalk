using System;
using Cirrious.MvvmCross.Plugins.Email;
using Cirrious.MvvmCross.Plugins.PhoneCall;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Client.Core.Model.DataContracts;
using SmartWalk.Client.Core.Resources;
using SmartWalk.Client.Core.Services;
using SmartWalk.Client.iOS.Utils.Map;

namespace SmartWalk.Client.iOS.Services
{
    public class EnvironmentService : IEnvironmentService
    {
        private readonly IMvxPhoneCallTask _phoneCallTask;
        private readonly IMvxComposeEmailTask _composeEmailTask;
        private readonly IReachabilityService _reachabilityService;

        public EnvironmentService(
            IMvxPhoneCallTask phoneCallTask,
            IMvxComposeEmailTask composeEmailTask,
            IReachabilityService reachabilityService)
        {
            _phoneCallTask = phoneCallTask;
            _composeEmailTask = composeEmailTask;
            _reachabilityService = reachabilityService;
        }

        public IReachabilityService Reachability
        {
            get { return _reachabilityService; }
        }

        public void Copy(string str)
        {
            UIPasteboard.General.String = str;
        }

        public string Paste()
        {
            return UIPasteboard.General.String;
        }

        public void MakePhoneCall(string name, string number)
        {
            _phoneCallTask.MakePhoneCall(name, number);
        }

        public void ComposeEmail(string to, string cc, string subject, string body, bool isHtml)
        {
            _composeEmailTask.ComposeEmail(to, cc, subject, body, isHtml);
        }

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

        public void ShowDirections(Address address)
        {
            MapUtil.OpenAddressInMaps(address);
        }

        public void Alert(string title, string message)
        {
            var alert = new UIAlertView(title, message, null, Localization.OK, null);
            alert.Show();
        }

        public void WriteConsoleLine(string line, params object[] arg)
        {
#if DEBUG
            Console.WriteLine(line, arg);
#endif
        }
    }
}