using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using GoogleAnalytics;
using SmartWalk.Client.Core.Services;
using SmartWalk.Client.Core.ViewModels;

namespace SmartWalk.Client.iOS.Services
{
    public class AnalyticsService : IAnalyticsService
    {
        public static bool IsOptOut { get; set; }

        private readonly IEnvironmentService _environmentService;

        public AnalyticsService(IEnvironmentService environmentService)
        {
            _environmentService = environmentService;
        }

        public void SendView(string name, Dictionary<string, object> parameters = null)
        {
            if (name == null) throw new ArgumentNullException("name");

            var viewPath = name + "/" + GetParametersString(parameters);

            if (!IsOptOut)
            {
                EasyTracker.GetTracker().SendView(viewPath);
            }

            _environmentService.WriteConsoleLine("GA: Send View - {0}", viewPath);
        }

        public void SendEvent(string category, string action, string label, int value = 0)
        {
            if (category == null) throw new ArgumentNullException("category");
            if (action == null) throw new ArgumentNullException("action");

            if (!IsOptOut)
            {
                EasyTracker.GetTracker().SendEvent(category, action, label, value);
            }

            _environmentService.WriteConsoleLine(
                "GA: Send Event - category:{0}, action:{1}, label:{2}, value:{3}",
                category,
                action,
                label,
                value);
        }

        public void SendException(bool isFatal, string format)
        {
            if (!IsOptOut)
            {
                EasyTracker.GetTracker().SendException(format, isFatal);
            }

            _environmentService.WriteConsoleLine("GA: Send Exception - {0}", format);
        }

        private static string GetParametersString(Dictionary<string, object> parameters)
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

                        var formattedValue = paramValue.ToString().ToLower();

                        if (paramValue is DateTime)
                        {
                            var dateValue = (DateTime)paramValue;
                            formattedValue = dateValue.ToShortDateString();
                        }

                        builder.Append(key + "=" + HttpUtility.HtmlEncode(formattedValue));
                    }
                }
            }

            if (builder.Length > 0)
            {
                var result = "?" + builder;
                return result;
            }

            return string.Empty;
        }
    }
}