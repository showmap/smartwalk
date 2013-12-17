using System;

namespace SmartWalk.Core.Services
{
    public interface IExceptionPolicy
    {
        void Trace(Exception ex);
    }
}