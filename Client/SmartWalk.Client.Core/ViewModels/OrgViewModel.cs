using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Cirrious.MvvmCross.Plugins.Email;
using Cirrious.MvvmCross.Plugins.PhoneCall;
using Cirrious.MvvmCross.ViewModels;
using SmartWalk.Client.Core.Model;
using SmartWalk.Client.Core.Services;
using SmartWalk.Client.Core.Utils;
using SmartWalk.Client.Core.ViewModels.Common;

namespace SmartWalk.Client.Core.ViewModels
{
    public class OrgViewModel : EntityViewModel
    {
        private readonly ISmartWalkApiService _apiService;
        private readonly IExceptionPolicy _exceptionPolicy;

        private Parameters _parameters;
        private Org _org;
        private ICommand _navigateOrgEventViewCommand;

        public OrgViewModel(
            IClipboard clipboard,
            IConfiguration configuration,
            ISmartWalkApiService apiService,
            IAnalyticsService analyticsService,
            IMvxPhoneCallTask phoneCallTask,
            IMvxComposeEmailTask composeEmailTask,
            IShowDirectionsTask showDirectionsTask,
            IExceptionPolicy exceptionPolicy) : 
                base(configuration,
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
                if (Org != null && Org.Info != null)
                {
                    return Org.Info.Name;
                }

                return null;
            }
        }

        public Org Org
        {
            get
            {
                return _org;
            }
            private set
            {
                if (!Equals(_org, value))
                {
                    _org = value;
                    Entity = _org != null ? _org.Info : null;
                    RaisePropertyChanged(() => Org);
                    RaisePropertyChanged(() => Title);
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
                                Location = _parameters.Location
                            }),
                        orgEvent => orgEvent != null);
                }

                return _navigateOrgEventViewCommand;
            }
        }

        protected override ParametersBase InitParameters
        {
            get { return _parameters; }
        }

        public void Init(Parameters parameters)
        {
            _parameters = parameters;

            UpdateOrg(DataSource.Cache).ContinueWithThrow();
        }

        protected override void Refresh()
        {
            UpdateOrg(DataSource.Server).ContinueWithThrow();
        }

        private async Task UpdateOrg(DataSource source)
        {
            if (_parameters != null)
            {
                IsLoading = true;

                var org = default(Org);

                try 
                {
                    org = await _apiService.GetHost(_parameters.OrgId, source);
                }
                catch (Exception ex)
                {
                    _exceptionPolicy.Trace(ex);
                }

                IsLoading = false;

                Org = org;
                RaiseRefreshCompleted();
            }
            else
            {
                Org = null;
            }
        }

        public class Parameters : ParametersBase
        {
            public int OrgId { get; set; }
        }
    }
}