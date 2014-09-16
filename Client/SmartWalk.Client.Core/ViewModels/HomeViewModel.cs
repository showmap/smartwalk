using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Cirrious.MvvmCross.ViewModels;
using SmartWalk.Shared.Utils;
using SmartWalk.Client.Core.Model;
using SmartWalk.Client.Core.Services;
using SmartWalk.Client.Core.Utils;
using SmartWalk.Client.Core.ViewModels.Common;

namespace SmartWalk.Client.Core.ViewModels
{
    public class HomeViewModel : RefreshableViewModel
    {
        private readonly ISmartWalkApiService _apiService;
        private readonly IExceptionPolicyService _exceptionPolicy;
        private readonly ILocationService _locationService;
        private readonly Parameters _parameters;

        private string _locationString;
        private OrgEvent[] _eventInfos;
        private ICommand _navigateOrgEventViewCommand;

        public HomeViewModel(
            ISmartWalkApiService apiService,
            IExceptionPolicyService exceptionPolicy,
            IAnalyticsService analyticsService,
            IReachabilityService reachabilityService,
            ILocationService locationService) 
            : base(reachabilityService, analyticsService)
        {
            _apiService = apiService;
            _exceptionPolicy = exceptionPolicy;
            _locationService = locationService;

            _parameters = new Parameters();

            _locationService.LocationChanged += (s, e) => Refresh(DataSource.Server);
            _locationService.LocationStringChanged += (s, e) => UpdateLocationString();

            UpdateLocationString();
        }

        public override string Title
        {
            get { return LocationString; }
        }

        public string LocationString
        {
            get
            {
                return _locationString;
            }
            private set
            {
                if (!Equals(_locationString, value))
                {
                    _locationString = value;
                    RaisePropertyChanged(() => LocationString);
                    RaisePropertyChanged(() => Title);

                    _parameters.Location = _locationString;
                }
            }
        }

        public OrgEvent[] EventInfos 
        {
            get
            {
                return _eventInfos;
            }
            private set
            {
                if (!_eventInfos.EnumerableEquals(value))
                {
                    _eventInfos = value;
                    RaisePropertyChanged(() => EventInfos);
                }
            }
        }

        public ICommand NavigateOrgEventViewCommand
        {
            get
            {
                if (_navigateOrgEventViewCommand == null)
                {
                    _navigateOrgEventViewCommand = new MvxCommand<OrgEvent>(
                        orgEvent => ShowViewModel<OrgEventViewModel>(
                            new OrgEventViewModel.Parameters { 
                                EventId = orgEvent.Id,
                                Current = true,
                                Location = _parameters.Location
                            }));
                }

                return _navigateOrgEventViewCommand;
            }
        }

        protected override ParametersBase InitParameters
        {
            get { return _parameters; }
        }

        public override void Start()
        {
            UpdateData(UpdateEventInfos);

            base.Start();
        }

        protected override void Refresh(DataSource source)
        {
            UpdateEventInfos(source).ContinueWithThrow();
        }

        private async Task<DataSource> UpdateEventInfos(DataSource source, bool showProgress = true)
        {
            if (showProgress) IsLoading = true;

            var eventInfos = default(OrgEvent[]);

            try
            {
                var result = await _apiService.GetOrgEvents(
                    _locationService.CurrentLocation,
                    source);
                if (result != null)
                {
                    eventInfos = result.Data;
                    source = result.Source;
                }
            }
            catch (Exception ex)
            {
                _exceptionPolicy.Trace(ex);
            }
                
            if (showProgress) IsLoading = false;

            if (eventInfos != null)
            {
                EventInfos = eventInfos;
            }

            if (showProgress)
            {
                RaiseRefreshCompleted(EventInfos != null);
            }

            return source;
        }

        private void UpdateLocationString()
        {
            LocationString = _locationService.CurrentLocationString;
        }

        public class Parameters : ParametersBase
        {
        }
    }
}