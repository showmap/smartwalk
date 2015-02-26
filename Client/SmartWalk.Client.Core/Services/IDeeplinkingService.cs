using System;

namespace SmartWalk.Client.Core.Services
{
    public interface IDeeplinkingService
    {
        bool NavigateView(Uri url);
    }
}