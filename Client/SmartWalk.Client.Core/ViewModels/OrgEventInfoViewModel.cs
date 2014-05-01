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
            IMvxPhoneCallTask phoneCallTask,
            IMvxComposeEmailTask composeEmailTask,
            IShowDirectionsTask showDirectionsTask,
            IExceptionPolicy exceptionPolicy) : 
                base(
                    configuration,
                    clipboard,
                    analyticsService, 
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
                    Entity = _orgEvent != null 
                        ? new Entity { 
                            Name = _orgEvent.Title, 
                            Description = _orgEvent.Description, 
                            Picture = _orgEvent.Picture
                        } 
                        : null;
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

        protected override void Refresh()
        {
            UpdateOrgEventInfo(DataSource.Server).ContinueWithThrow();
        }

        private async Task UpdateOrgEventInfo(DataSource source)
        {
            if (_parameters != null)
            {
                IsLoading = true;

                var orgEvent = default(OrgEvent);

                try 
                {
                    orgEvent = await _apiService.GetOrgEventInfo(_parameters.OrgEventId, source);
                }
                catch (Exception ex)
                {
                    _exceptionPolicy.Trace(ex);
                }

                IsLoading = false;

                OrgEvent = orgEvent;
                RaiseRefreshCompleted();
            }
            else
            {
                OrgEvent = null;
            }
        }

        public class Parameters : ParametersBase
        {
            public int OrgEventId { get; set; }
        }
    }
}