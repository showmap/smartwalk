using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Cirrious.MvvmCross.Plugins.Email;
using Cirrious.MvvmCross.Plugins.PhoneCall;
using Cirrious.MvvmCross.ViewModels;
using SmartWalk.Client.Core.Model;
using SmartWalk.Client.Core.Services;
using SmartWalk.Client.Core.Utils;

namespace SmartWalk.Client.Core.ViewModels
{
    public class OrgViewModel : EntityViewModel
    {
        private readonly ISmartWalkDataService _dataService;
        private readonly IExceptionPolicy _exceptionPolicy;

        private Parameters _parameters;
        private EntityInfo[] _orgInfos;
        private ICommand _navigateOrgEventViewCommand;

        public OrgViewModel(
            ISmartWalkDataService dataService,
            IAnalyticsService analyticsService,
            IMvxPhoneCallTask phoneCallTask,
            IMvxComposeEmailTask composeEmailTask,
            IShowDirectionsTask showDirectionsTask,
            IExceptionPolicy exceptionPolicy) : 
                base(analyticsService, phoneCallTask, composeEmailTask, showDirectionsTask)
        {
            _dataService = dataService;
            _exceptionPolicy = exceptionPolicy;
        }

        public Org Org
        {
            get
            {
                return (Org)Entity;
            }
            private set
            {
                if (!Equals(Entity, value))
                {
                    Entity = value;
                    RaisePropertyChanged(() => Org);
                }
            }
        }

        public EntityInfo[] OrgInfos
        {
            get
            {
                return _orgInfos;
            }
            private set
            {
                if (!Equals(_orgInfos, value))
                {
                    _orgInfos = value;
                    RaisePropertyChanged(() => OrgInfos);
                }
            }
        }

        public ICommand NavigateOrgEventViewCommand
        {
            get
            {
                if (_navigateOrgEventViewCommand == null)
                {
                    _navigateOrgEventViewCommand = new MvxCommand<OrgEventInfo>(
                        evenInfo => ShowViewModel<OrgEventViewModel>(
                        new OrgEventViewModel.Parameters {  
                            OrgId = evenInfo.OrgId, 
                            Date = evenInfo.Date
                        }),
                        eventInfo => eventInfo.HasSchedule);
                }

                return _navigateOrgEventViewCommand;
            }
        }

        public override bool CanShowNextEntity
        {
            get { return OrgInfos != null && OrgInfos.Length > 0 && Org != null; }
        }

        public override bool CanShowPreviousEntity
        {
            get { return CanShowNextEntity; }
        }

        protected override object InitParameters
        {
            get { return _parameters; }
        }

        public void Init(Parameters parameters)
        {
            _parameters = parameters;

            UpdateOrg().ContinueWithThrow();
            UpdateOrgInfos().ContinueWithThrow();
        }

        protected override void Refresh()
        {
            UpdateOrg().ContinueWithThrow();
        }

        protected override void OnShowPreviousEntity()
        {
            var currentOrg = OrgInfos.FirstOrDefault(oi => oi.Id == Org.Info.Id);
            var currentIndex = Array.IndexOf(OrgInfos, currentOrg);
            if (currentIndex > 0)
            {
                _parameters.OrgId = OrgInfos[currentIndex - 1].Id;
            }
            else
            {
                _parameters.OrgId = OrgInfos.Last().Id;
            }

            UpdateOrg(DataSource.Cache).ContinueWithThrow();
        }

        protected override void OnShowNextEntity()
        {
            var currentOrg = OrgInfos.FirstOrDefault(oi => oi.Id == Org.Info.Id);
            var currentIndex = Array.IndexOf(OrgInfos, currentOrg);
            if (currentIndex < OrgInfos.Length - 1)
            {
                _parameters.OrgId = OrgInfos[currentIndex + 1].Id;
            }
            else
            {
                _parameters.OrgId = OrgInfos[0].Id;
            }

            UpdateOrg(DataSource.Cache).ContinueWithThrow();
        }

        private async Task UpdateOrg(DataSource source = DataSource.Server)
        {
            if (_parameters != null)
            {
                IsLoading = true;

                var org = default(Org);

                try 
                {
                    org = await _dataService.GetOrg(_parameters.OrgId, source);
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

        private async Task UpdateOrgInfos()
        {
            if (_parameters != null)
            {
                var index = default(LocationIndex);

                try 
                {
                    index = await _dataService.GetLocationIndex(DataSource.Cache);
                }
                catch (Exception ex)
                {
                    _exceptionPolicy.Trace(ex);
                }

                OrgInfos = index != null ? index.OrgInfos : null;
                RaiseRefreshCompleted();
            }
            else
            {
                OrgInfos = null;
            }
        }

        public class Parameters
        {
            public string OrgId { get; set; }
        }
    }
}