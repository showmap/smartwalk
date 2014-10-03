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
    public abstract class RefreshableViewModel : ProgressViewModel, IRefreshableViewModel, ITitleAware
    {
        private const string KeyPrefix = "refresh";

        private readonly IReachabilityService _reachabilityService;
        private readonly IAnalyticsService _analyticsService;
        private readonly IPostponeService _postponeService;

        private ICommand _refreshCommand;

        protected RefreshableViewModel(
            IReachabilityService reachabilityService,
            IAnalyticsService analyticsService,
            IPostponeService postponeService) 
            : base(analyticsService)
        {
            _reachabilityService = reachabilityService;
            _analyticsService = analyticsService;
            _postponeService = postponeService;
        }

        public event EventHandler<MvxValueEventArgs<bool>>  RefreshCompleted;

        public ICommand RefreshCommand
        {
            get 
            {
                if (_refreshCommand == null)
                {
                    _refreshCommand = new MvxCommand(() => { 
                        // invalidate postpone wait one manual refresh from server
                        var key = GenerateKey(InitParameters);
                        _postponeService.Invalidate(key);

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

        protected void UpdateData(
            Func<DataSource, bool, Task<DataSource>> updateHandler, 
            bool usePostponing = true)
        {
            // firstly loading from cache or server
            updateHandler(DataSource.CacheOrServer, true)
                .ContinueWithUIThread(previous => {
                    var key = GenerateKey(InitParameters);

                    // if data is from cache and it's been a while
                    // then loading from server in the background for a quite refresh
                    if (previous.Result == DataSource.Cache &&
                        (!usePostponing || !_postponeService.ShouldPostpone(key))) 
                    { 
                        updateHandler(DataSource.Server, false);
                    }
                })
                .ContinueWithThrow();
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
                Refresh(DataSource.CacheOrServer);
            }
        }

        private static string GenerateKey(ParametersBase parameters)
        {
            return KeyPrefix + 
                (parameters != null 
                    ? parameters.GetHashCode().ToString() 
                    : string.Empty);
        }
    }
}