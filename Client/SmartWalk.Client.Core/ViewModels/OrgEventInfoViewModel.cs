using System;
using System.Threading.Tasks;
using SmartWalk.Client.Core.Model;
using SmartWalk.Client.Core.Model.DataContracts;
using SmartWalk.Client.Core.Services;
using SmartWalk.Client.Core.Utils;
using SmartWalk.Client.Core.ViewModels.Common;
using SmartWalk.Shared.Utils;

namespace SmartWalk.Client.Core.ViewModels
{
    public class OrgEventInfoViewModel : EntityViewModel
    {
        private readonly ISmartWalkApiService _apiService;
        private readonly IExceptionPolicyService _exceptionPolicy;

        private Parameters _parameters;
        private OrgEvent _orgEvent;

        public OrgEventInfoViewModel(
            IEnvironmentService environmentService,
            IConfiguration configuration,
            ISmartWalkApiService apiService,
            IAnalyticsService analyticsService,
            IExceptionPolicyService exceptionPolicy,
            IPostponeService postponeService) : 
                base(
                    configuration,
                    environmentService,
                    analyticsService,
                    postponeService)
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

            UpdateData(UpdateOrgEventInfo);
        }

        protected override void Refresh(DataSource source)
        {
            UpdateOrgEventInfo(source).ContinueWithThrow();
        }

        private async Task<DataSource> UpdateOrgEventInfo(DataSource source, bool showProgress = true)
        {
            if (_parameters != null)
            {
                if (showProgress) IsLoading = true;

                var orgEvent = default(OrgEvent);

                try 
                {
                    var result = await _apiService
                        .GetOrgEventInfo(_parameters.EventId, source);
                    if (result != null)
                    {
                        orgEvent = result.Data;
                        source = result.Source;
                    }
                }
                catch (Exception ex)
                {
                    _exceptionPolicy.Trace(ex);
                }

                if (showProgress) IsLoading = false;

                if (orgEvent != null)
                {
                    OrgEvent = orgEvent;
                }

                if (showProgress)
                {
                    RaiseRefreshCompleted(OrgEvent != null);
                }
            }
            else
            {
                OrgEvent = null;
            }

            return source;
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

            public override bool Equals(object obj)
            {
                var parameters = obj as Parameters;
                if (parameters != null)
                {
                    return Equals(Location, parameters.Location) && 
                        EventId == parameters.EventId;
                }

                return false;
            }

            public override int GetHashCode()
            {
                return HashCode.Initial
                    .CombineHashCodeOrDefault(Location)
                    .CombineHashCode(EventId);
            }
        }
    }
}