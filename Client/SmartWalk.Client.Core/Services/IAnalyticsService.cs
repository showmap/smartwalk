using System.Collections.Generic;

namespace SmartWalk.Client.Core.Services
{
    public interface IAnalyticsService
    {
        void SendView(string name, Dictionary<string, object> parameters = null);

        void SendEvent(string category, string action, string label, int value = 0);

        void SendException(bool isFatal, string format);
    }
}