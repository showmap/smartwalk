using System;
using System.ComponentModel;
using System.Windows.Input;
using Cirrious.MvvmCross.ViewModels;

namespace SmartWalk.Core.ViewModels
{
    public interface IRefreshableViewModel : IMvxViewModel, INotifyPropertyChanged
    {
        event EventHandler RefreshCompleted;

        ICommand RefreshCommand { get; }
    }
}