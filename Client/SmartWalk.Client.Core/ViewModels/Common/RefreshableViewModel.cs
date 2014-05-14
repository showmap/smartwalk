using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Cirrious.CrossCore.Core;
using Cirrious.MvvmCross.ViewModels;
using SmartWalk.Client.Core.Constants;
using SmartWalk.Client.Core.Services;
using SmartWalk.Client.Core.Utils;
using SmartWalk.Client.Core.ViewModels.Interfaces;

namespace SmartWalk.Client.Core.ViewModels.Common
{
    public abstract class RefreshableViewModel : ProgressViewModel, IRefreshableViewModel
    {
        private readonly IReachabilityService _reachabilityService;
        private readonly IAnalyticsService _analyticsService;

        private ICommand _refreshCommand;

        protected RefreshableViewModel(
            IReachabilityService reachabilityService,
            IAnalyticsService analyticsService) 
            : base(analyticsService)
        {
            _reachabilityService = reachabilityService;
            _analyticsService = analyticsService;
        }

        public event EventHandler<MvxValueEventArgs<bool>>  RefreshCompleted;

        public ICommand RefreshCommand
        {
            get 
            {
                if (_refreshCommand == null)
                {
                    _refreshCommand = new MvxCommand(() => { 
                        Refresh(DataSource.Server);

                        _analyticsService.SendEvent(
                            Analytics.CategoryUI,
                            Analytics.ActionPull,
                            Analytics.ActionLabelRefresh);
                    });
                }

                return _refreshCommand;
            }
        }

        public abstract string Title { get; }

        protected abstract void Refresh(DataSource source);

        protected override void OnActivate()
        {
            base.OnActivate();

            _reachabilityService.StateChanged += OnReachableStateChanged;
        }

        protected override void OnDeactivate()
        {
            base.OnDeactivate();

            _reachabilityService.StateChanged -= OnReachableStateChanged;
        }

        protected void RaiseRefreshCompleted(bool hasData)
        {
            if (RefreshCompleted != null)
            {
                RefreshCompleted(this, new MvxValueEventArgs<bool>(hasData));
            }
        }

        private void OnReachableStateChanged(object sender, EventArgs e)
        {
            RefreshIfReachable().ContinueWithThrow();
        }

        private async Task RefreshIfReachable()
        {
            var isReachable = await _reachabilityService.GetIsReachable();
            if (isReachable)
            {
                Refresh(DataSource.Cache);
            }
        }
    }
}