using System;
using System.Linq;
using System.Windows.Input;
using Cirrious.MvvmCross.ViewModels;
using SmartWalk.Core.Model;
using SmartWalk.Core.Services;

namespace SmartWalk.Core.ViewModels
{
    public class VenueViewModel : EntityViewModel
    {
        private readonly ISmartWalkDataService _dataService;
        private readonly IExceptionPolicy _exceptionPolicy;

        private Parameters _parameters;
        private ICommand _refreshCommand;

        public VenueViewModel(ISmartWalkDataService dataService, IExceptionPolicy exceptionPolicy)
        {
            _dataService = dataService;
            _exceptionPolicy = exceptionPolicy;
        }

        public Venue Venue
        {
            get
            {
                return (Venue)Entity;
            }
            private set
            {
                if (!Equals(Entity, value))
                {
                    Entity = value;
                    RaisePropertyChanged(() => Venue);
                }
            }
        }

        public ICommand RefreshCommand
        {
            get 
            {
                if (_refreshCommand == null)
                {
                    _refreshCommand = new MvxCommand(UpdateVenue);
                }

                return _refreshCommand;
            }
        }

        public void Init(Parameters parameters)
        {
            _parameters = parameters;

            UpdateVenue();
        }

        private void UpdateVenue()
        {
            if (_parameters != null)
            {
                _dataService.GetOrgEvent(
                    _parameters.OrgId,
                    _parameters.EventDate,
                    DataSource.Cache,
                    (orgEvent, ex) => 
                        {
                            if (ex == null)
                            {
                                Venue = orgEvent.Venues.FirstOrDefault(v => 
                                   v.Number == _parameters.VenueNumber &&
                                   string.Compare(v.Info.Name, _parameters.VenueName, true) == 0);
                            }
                            else
                            {
                                _exceptionPolicy.Trace(ex);
                            }
                        });
            }
            else
            {
                Venue = null;
            }
        }

        public class Parameters
        {
            public string OrgId { get; set; }
            public DateTime EventDate { get; set; }
            public int VenueNumber { get; set; }
            public string VenueName { get; set; }
        }
    }
}