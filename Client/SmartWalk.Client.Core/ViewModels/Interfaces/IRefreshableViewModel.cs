using System;
using System.ComponentModel;
using System.Windows.Input;
using Cirrious.CrossCore.Core;
using Cirrious.MvvmCross.ViewModels;

namespace SmartWalk.Client.Core.ViewModels.Interfaces
{
    public interface IRefreshableViewModel : IMvxViewModel, INotifyPropertyChanged
    {
        event EventHandler<MvxValueEventArgs<bool>>  RefreshCompleted;

        ICommand RefreshCommand { get; }

        string Title { get; }
    }
}