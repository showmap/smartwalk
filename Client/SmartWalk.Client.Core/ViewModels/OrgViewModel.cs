using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Cirrious.MvvmCross.ViewModels;
using SmartWalk.Client.Core.Model;
using SmartWalk.Client.Core.Services;
using SmartWalk.Client.Core.Utils;
using SmartWalk.Client.Core.ViewModels.Common;
using SmartWalk.Shared.Utils;

namespace SmartWalk.Client.Core.ViewModels
{
    public class OrgViewModel : EntityViewModel
    {
        private readonly ISmartWalkApiService _apiService;
        private readonly IExceptionPolicyService _exceptionPolicy;

        private Parameters _parameters;
        private Org _org;
        private ICommand _navigateOrgEventViewCommand;

        public OrgViewModel(
            IEnvironmentService environmentService,
            IConfiguration configuration,
            ISmartWalkApiService apiService,
            IAnalyticsService analyticsService,
            IExceptionPolicyService exceptionPolicy,
            IPostponeService postponeService) : 
                base(configuration,
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

            UpdateData(UpdateOrg);
        }

        protected override void Refresh(DataSource source)
        {
            UpdateOrg(source).ContinueWithThrow();
        }

        private async Task<DataSource> UpdateOrg(DataSource source, bool showProgress = true)
        {
            if (_parameters != null)
            {
                if (showProgress) IsLoading = true;

                var org = default(Org);

                try 
                {
                    var result = await _apiService.GetHost(_parameters.OrgId, source);
                    if (result != null)
                    {
                        org = result.Data;
                        source = result.Source;
                    }
                }
                catch (Exception ex)
                {
                    _exceptionPolicy.Trace(ex);
                }

                if (showProgress) IsLoading = false;

                if (org != null)
                {
                    Org = org;
                }

                if (showProgress)
                {
                    RaiseRefreshCompleted(Org != null);
                }
            }
            else
            {
                Org = null;
            }

            return source;
        }

        public class Parameters : ParametersBase
        {
            public int OrgId { get; set; }

            public override bool Equals(object obj)
            {
                var parameters = obj as Parameters;
                if (parameters != null)
                {
                    return Equals(Location, parameters.Location) && 
                        OrgId == parameters.OrgId;
                }

                return false;
            }

            public override int GetHashCode()
            {
                return HashCode.Initial
                    .CombineHashCodeOrDefault(Location)
                    .CombineHashCode(OrgId);
            }
        }
    }
}