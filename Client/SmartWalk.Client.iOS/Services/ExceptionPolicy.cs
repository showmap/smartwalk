using System;
using System.Net;
using System.Threading.Tasks;
using MonoTouch.UIKit;
using Newtonsoft.Json;
using SmartWalk.Client.Core.Services;
using SmartWalk.Client.iOS.Resources;

namespace SmartWalk.Client.iOS.Services
{
    public class ExceptionPolicy : IExceptionPolicy
    {
        private readonly IAnalyticsService _analyticsService;

        public ExceptionPolicy(IAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        public void Trace(Exception ex, bool showAlert = true)
        {
            var sendToGA = false;

            if (showAlert)
            {
                string title = Localization.Error;
                string message = ex.Message;

                if (ex is WebException)
                {
                    title = Localization.NetworkError;
                    message = Localization.CantAccessNetworkContent;
                }
                else if (ex is JsonReaderException)
                {
                    title = Localization.ServerError;
                    message = Localization.CantReadNetworkContent;
                    sendToGA = true;
                }
                else if (ex is TaskCanceledException)
                {
                    return;
                }
                else
                {
                    sendToGA = true;
                }

                var alert = new UIAlertView(title, message, null, Localization.OK, null);
                alert.Show();
            }

            if (sendToGA)
            {
                _analyticsService.SendException(false, ex.ToString());
            }
        }
    }
}