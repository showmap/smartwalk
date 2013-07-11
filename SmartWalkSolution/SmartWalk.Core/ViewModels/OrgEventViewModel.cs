using System;
using System.Windows.Input;
using Cirrious.MvvmCross.ViewModels;
using SmartWalk.Core.Model;
using SmartWalk.Core.Services;

namespace SmartWalk.Core.ViewModels
{
    public class OrgEventViewModel : MvxViewModel
    {
        private readonly ISmartWalkDataService _dataService;

        private OrgEvent _orgEvent;
        private ICommand _refreshCommand;
        private Parameters _parameters;

        public OrgEventViewModel(ISmartWalkDataService dataService)
        {
            _dataService = dataService;
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
                    RaisePropertyChanged(() => OrgEvent);
                }
            }
        }

        public ICommand RefreshCommand
        {
            get 
            {
                if (_refreshCommand == null)
                {
                    _refreshCommand = new MvxCommand(UpdateOrgEvent);
                }

                return _refreshCommand;
            }
        }

        public void Init(Parameters parameters)
        {
            _parameters = parameters;

            UpdateOrgEvent();
        }

        private void UpdateOrgEvent()
        {
            if (_parameters != null)
            {
                _dataService.GetOrgEvent(_parameters.OrgId, _parameters.Date, (orgEvent, ex) => 
                    {
                        if (ex == null)
                        {
                            OrgEvent = orgEvent;
                        }
                        else
                        {
                            // TODO: handling
                        }
                    });
            }
            else
            {
                OrgEvent = null;
            }
        }

        public class Parameters
        {
            public string OrgId { get; set; }

            public DateTime Date { get; set; }
        }
    }
}