using System;
using SmartWalk.Client.Core.Model;

namespace SmartWalk.Client.Core.Services
{
    public interface ILocationService
    {
        event EventHandler LocationChanged;
        event EventHandler LocationStringChanged;

        Location CurrentLocation { get; }
        string CurrentLocationString { get; }
    }
}