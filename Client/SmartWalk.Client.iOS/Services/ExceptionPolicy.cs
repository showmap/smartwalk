using System;
using System.Net;
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
            _analyticsService.SendException(false, ex.ToString());

            if (showAlert)
            {
                string title = Localization.Error;
                string message = ex.Message;

                if (ex is WebException)
                {
                    title = Localization.NetworkError;
                    message = Localization.CantAccessNetworkContent;
                }

                if (ex is JsonReaderException)
                {
                    title = Localization.ServerError;
                    message = Localization.CantReadNetworkContent;
                }

                var alert = new UIAlertView(title, message, null, "OK", null);
                alert.Show();
            }
        }
    }
}