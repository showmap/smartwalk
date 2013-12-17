using System;

namespace SmartWalk.Core.Services
{
    public interface ILocationService
    {
        event EventHandler LocationChanged;

        string CurrentLocation { get; }
    }
}