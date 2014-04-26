using System;
using System.Windows.Input;
using Cirrious.CrossCore.Core;

namespace SmartWalk.Client.Core.ViewModels.Interfaces
{
    public interface IShareableViewModel
    {
        event EventHandler<MvxValueEventArgs<string>> Share;

        ICommand ShareCommand { get; }
    }
}