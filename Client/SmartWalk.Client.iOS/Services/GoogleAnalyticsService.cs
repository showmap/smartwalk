using System;
using System.Collections.Generic;
using System.Text;
using GoogleAnalytics;
using SmartWalk.Client.Core.Services;
using SmartWalk.Client.Core.Utils;
using SmartWalk.Client.Core.ViewModels;

namespace SmartWalk.Client.iOS.Services
{
    public class GoogleAnalyticsService : IAnalyticsService
    {
        private readonly ILocationService _locationService;

        public GoogleAnalyticsService(ILocationService locationService)
        {
            _locationService = locationService;
        }

        public bool IsOptOut
        {
            get { return GAI.SharedInstance.OptOut; }
            set { GAI.SharedInstance.OptOut = value; }
        }

        public void SendView(string name, Dictionary<string, object> parameters = null)
        {
            if (name == null) throw new ArgumentNullException("name");

            var viewPath = name + "/" + GetParametersString(parameters);

            GAI.SharedInstance.DefaultTracker.SendView(viewPath);
            ConsoleUtil.Log("GA: Send View - {0}", viewPath);
        }

        public void SendEvent(string category, string action, string label, int value = 0)
        {
            if (category == null) throw new ArgumentNullException("category");
            if (action == null) throw new ArgumentNullException("action");

            GAI.SharedInstance.DefaultTracker.SendEvent(category, action, label, value);
            ConsoleUtil.Log(
                "GA: Send Event - category:{0}, action:{1}, label:{2}, value:{3}",
                category,
                action,
                label,
                value);
        }

        public void SendException(bool isFatal, string format)
        {
            GAI.SharedInstance.DefaultTracker.SendException(isFatal, format);
            ConsoleUtil.Log("GA: Send Exception - {0}", format);
        }

        private string GetParametersString(Dictionary<string, object> parameters)
        {
            var builder = new StringBuilder();

            if (parameters != null)
            {
                foreach (var key in parameters.Keys)
                {
                    var paramValue = parameters[key];
                    if (paramValue != null && 
                        !(paramValue is Addresses))
                    {
                        if (builder.Length > 0)
                        {
                            builder.Append("&");
                        }

                        var formattedValue = paramValue.ToString();

                        if (paramValue is DateTime)
                        {
                            var dateValue = (DateTime)paramValue;
                            formattedValue = dateValue.ToShortDateString();
                        }

                        builder.Append(key + "=" + formattedValue);
                    }
                }
            }

            var locationParam = _locationService.CurrentLocation != null 
                ? "loc=" + _locationService.CurrentLocation 
                : string.Empty;

            if (locationParam != string.Empty || builder.Length > 0)
            {
                var result = "?" + locationParam + 
                    (builder.Length > 0 
                        ? ((locationParam != null ? "&" : string.Empty) + builder) 
                        : string.Empty);
                return result;
            }

            return string.Empty;
        }
    }
}