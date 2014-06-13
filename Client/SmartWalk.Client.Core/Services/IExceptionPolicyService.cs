using System;

namespace SmartWalk.Client.Core.Services
{
    public interface IExceptionPolicyService
    {
        void Trace(Exception ex, bool showAlert = true);
    }
}