using System;
using System.Windows.Input;
using Cirrious.MvvmCross.ViewModels;
using SmartWalk.Client.Core.Constants;
using SmartWalk.Client.Core.Services;
using SmartWalk.Client.Core.ViewModels.Interfaces;

namespace SmartWalk.Client.Core.ViewModels.Common
{
    public abstract class RefreshableViewModel : ProgressViewModel, IRefreshableViewModel
    {
        private ICommand _refreshCommand;

        private readonly IAnalyticsService _analyticsService;

        protected RefreshableViewModel(IAnalyticsService analyticsService) : base(analyticsService)
        {
            _analyticsService = analyticsService;
        }

        public event EventHandler RefreshCompleted;

        public ICommand RefreshCommand
        {
            get 
            {
                if (_refreshCommand == null)
                {
                    _refreshCommand = new MvxCommand(() => { 
                        Refresh();

                        _analyticsService.SendEvent(
                            Analytics.CategoryUI,
                            Analytics.ActionPull,
                            Analytics.ActionLabelRefresh);
                    });
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