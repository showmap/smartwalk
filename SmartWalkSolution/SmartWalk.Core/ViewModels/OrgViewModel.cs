using System;
using System.Linq;
using System.Windows.Input;
using Cirrious.MvvmCross.ViewModels;
using SmartWalk.Core.Model;
using SmartWalk.Core.Services;
using SmartWalk.Core.ViewModels.Interfaces;

namespace SmartWalk.Core.ViewModels
{
    public class OrgViewModel : EntityViewModel, IRefreshableViewModel
    {
        private readonly ISmartWalkDataService _dataService;
        private readonly IExceptionPolicy _exceptionPolicy;

        private Parameters _parameters;
        private EntityInfo[] _orgInfos;
        private ICommand _refreshCommand;
        private ICommand _navigateOrgEventViewCommand;

        public OrgViewModel(ISmartWalkDataService dataService, IExceptionPolicy exceptionPolicy)
        {
            _dataService = dataService;
            _exceptionPolicy = exceptionPolicy;
        }

        public event EventHandler RefreshCompleted;

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

        public ICommand RefreshCommand
        {
            get 
            {
                if (_refreshCommand == null)
                {
                    _refreshCommand = new MvxCommand(() => UpdateOrg());
                }

                return _refreshCommand;
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

        public void Init(Parameters parameters)
        {
            _parameters = parameters;

            UpdateOrg();
            UpdateOrgInfos();
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

            UpdateOrg(DataSource.Cache);
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
                _parameters.OrgId = OrgInfos.First().Id;
            }

            UpdateOrg(DataSource.Cache);
        }

        private void UpdateOrg(DataSource source = DataSource.Server)
        {
            if (_parameters != null)
            {
                _dataService.GetOrg(_parameters.OrgId, source, (org, ex) => 
                    {
                        if (ex == null)
                        {
                            Org = org;

                            if (RefreshCompleted != null)
                            {
                                RefreshCompleted(this, EventArgs.Empty);
                            }
                        }
                        else
                        {
                            _exceptionPolicy.Trace(ex);
                        }
                    });
            }
            else
            {
                Org = null;
            }
        }

        private void UpdateOrgInfos()
        {
            if (_parameters != null)
            {
                _dataService.GetLocation(
                    _parameters.Location, 
                    DataSource.Cache,
                    (location, ex) => 
                        {
                            if (ex == null)
                            {
                                OrgInfos = location != null ? location.OrgInfos : null;

                                if (RefreshCompleted != null)
                                {
                                    RefreshCompleted(this, EventArgs.Empty);
                                }
                            }
                            else
                            {
                                _exceptionPolicy.Trace(ex);
                            }
                        });
            }
            else
            {
                OrgInfos = null;
            }
        }

        public class Parameters
        {
            public string OrgId { get; set; }
            public string Location { get; set; }
        }
    }
}