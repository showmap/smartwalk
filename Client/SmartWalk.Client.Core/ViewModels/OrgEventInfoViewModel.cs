using System;
using System.Threading.Tasks;
using Cirrious.MvvmCross.Plugins.Email;
using Cirrious.MvvmCross.Plugins.PhoneCall;
using SmartWalk.Client.Core.Model;
using SmartWalk.Client.Core.Model.DataContracts;
using SmartWalk.Client.Core.Services;
using SmartWalk.Client.Core.Utils;
using SmartWalk.Client.Core.ViewModels.Common;

namespace SmartWalk.Client.Core.ViewModels
{
    public class OrgEventInfoViewModel : EntityViewModel
    {
        private readonly ISmartWalkApiService _apiService;
        private readonly IExceptionPolicy _exceptionPolicy;

        private Parameters _parameters;
        private OrgEvent _orgEvent;

        public OrgEventInfoViewModel(
            IClipboard clipboard,
            IConfiguration configuration,
            ISmartWalkApiService apiService,
            IAnalyticsService analyticsService,
            IReachabilityService reachabilityService,
            IMvxPhoneCallTask phoneCallTask,
            IMvxComposeEmailTask composeEmailTask,
            IShowDirectionsTask showDirectionsTask,
            IExceptionPolicy exceptionPolicy) : 
                base(
                    configuration,
                    clipboard,
                    analyticsService, 
                    reachabilityService,
                    phoneCallTask, 
                    composeEmailTask, 
                    showDirectionsTask)
        {
            _apiService = apiService;
            _exceptionPolicy = exceptionPolicy;
        }

        public override string Title
        {
            get
            {
                return OrgEvent != null ? OrgEvent.Title : null;

            }
        }

        public override string Subtitle
        {
            get
            {
                return OrgEvent.GetDateString();
            }
        }

        public OrgEvent OrgEvent
        {
            get
            {
                return _orgEvent;
            }
            private set
            {
                if (!Equals(_orgEvent, value))
                {
                    _orgEvent = value;

                    Entity = _orgEvent != null ? CreateEntity() : null;
                    IsDescriptionExpanded = true;

                    RaisePropertyChanged(() => OrgEvent);
                    RaisePropertyChanged(() => Title);
                }
            }
        }

        protected override ParametersBase InitParameters
        {
            get { return _parameters; }
        }

        public void Init(Parameters parameters)
        {
            _parameters = parameters;

            UpdateOrgEventInfo(DataSource.Cache).ContinueWithThrow();
        }

        protected override void Refresh(DataSource source)
        {
            UpdateOrgEventInfo(source).ContinueWithThrow();
        }

        private async Task UpdateOrgEventInfo(DataSource source)
        {
            if (_parameters != null)
            {
                IsLoading = true;

                var orgEvent = default(OrgEvent);

                try 
                {
                    orgEvent = await _apiService
                        .GetOrgEventInfo(_parameters.EventId, source);
                }
                catch (Exception ex)
                {
                    _exceptionPolicy.Trace(ex);
                }

                IsLoading = false;

                OrgEvent = orgEvent;
                RaiseRefreshCompleted(OrgEvent != null);
            }
            else
            {
                OrgEvent = null;
            }
        }

        private Entity CreateEntity()
        {
            var result = new Entity { 
                Name = _orgEvent.Title, 
                Description = _orgEvent.Description, 
                Picture = _orgEvent.Picture
            };

            if (_orgEvent.Latitude.HasValue && 
                _orgEvent.Longitude.HasValue)
            {
                result.Addresses = new [] {
                    new Address 
                    { 
                        AddressText = _orgEvent.Title,
                        Latitude = _orgEvent.Latitude.Value,
                        Longitude = _orgEvent.Longitude.Value
                    }
                };
            }

            return result;
        }

        public class Parameters : ParametersBase
        {
            public int EventId { get; set; }
        }
    }
}