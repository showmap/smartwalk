using System;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SmartWalk.Client.Core.Resources;
using SmartWalk.Client.Core.Services;

namespace SmartWalk.Client.Core.Services
{
    public class ExceptionPolicyService : IExceptionPolicyService
    {
        private readonly IEnvironmentService _environmentService;
        private readonly IAnalyticsService _analyticsService;

        public ExceptionPolicyService(
            IEnvironmentService environmentService,
            IAnalyticsService analyticsService)
        {
            _environmentService = environmentService;
            _analyticsService = analyticsService;
        }

        public void Trace(Exception ex, bool showAlert = true)
        {
            var sendToGA = false;

            string title = Localization.Error;
            string message = ex.Message;

            if (ex is WebException)
            {
                showAlert = false;
            }
            else if (ex is JsonReaderException)
            {
                title = Localization.ServerError;
                message = Localization.CantReadNetworkContent;
                sendToGA = true;
            }
            else if (ex is TaskCanceledException)
            {
                showAlert = false;
            }
            else
            {
                sendToGA = true;
            }

            _environmentService.WriteConsoleLine(ex.ToString());

            if (showAlert)
            {
                _environmentService.Alert(title, message);
            }

            if (sendToGA)
            {
                _analyticsService.SendException(false, ex.ToString());
            }
        }
    }
}