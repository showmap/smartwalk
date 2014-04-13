using System;

namespace SmartWalk.Client.Core.Services
{
    public interface IExceptionPolicy
    {
        void Trace(Exception ex, bool showAlert = true);
    }
}