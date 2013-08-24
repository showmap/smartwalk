using System;
using SmartWalk.Core.ViewModels.Interfaces;
using System.Windows.Input;
using Cirrious.MvvmCross.ViewModels;

namespace SmartWalk.Core.ViewModels.Common
{
    public abstract class RefreshableViewModel : ProgressViewModel, IRefreshableViewModel
    {
        private ICommand _refreshCommand;

        public event EventHandler RefreshCompleted;

        public ICommand RefreshCommand
        {
            get 
            {
                if (_refreshCommand == null)
                {
                    _refreshCommand = new MvxCommand(Refresh);
                }

                return _refreshCommand;
            }
        }

        protected abstract void Refresh();

        protected void RaiseRefreshCompleted()
        {
            if (RefreshCompleted != null)
            {
                RefreshCompleted(this, EventArgs.Empty);
            }
        }
    }
}