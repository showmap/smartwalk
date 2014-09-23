using System;
using SmartWalk.Client.Core.Model;
using SmartWalk.Client.Core.ViewModels.Interfaces;

namespace SmartWalk.Client.Core.Services
{
    public interface ILocationService : IActiveAware
    {
        event EventHandler LocationChanged;
        event EventHandler LocationStringChanged;

        Location CurrentLocation { get; }
        string CurrentLocationString { get; }

        void ResolveLocationIssues();
    }
}