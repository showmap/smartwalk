using System;

namespace SmartWalk.Client.Core.Services
{
    public interface ILocationService
    {
        event EventHandler LocationChanged;

        string CurrentLocation { get; }
    }
}