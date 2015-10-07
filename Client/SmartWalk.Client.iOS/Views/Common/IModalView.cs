using System;
using SmartWalk.Client.iOS.Views.Common.Base;

namespace SmartWalk.Client.iOS.Views.Common
{
    public interface IModalView : IDisposable
    {
        event EventHandler ToHide;

        ViewBase PresentingViewController { get; }
    }
}