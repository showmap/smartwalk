using System;
using System.Linq;
using System.Windows.Input;
using Cirrious.MvvmCross.ViewModels;
using SmartWalk.Core.Model;
using SmartWalk.Core.Services;

namespace SmartWalk.Core.ViewModels
{
    public class VenueViewModel : EntityViewModel, IRefreshableViewModel
    {
        private readonly ISmartWalkDataService _dataService;
        private readonly IExceptionPolicy _exceptionPolicy;

        private Parameters _parameters;
        private ICommand _refreshCommand;
        private MvxCommand<VenueShow> _expandCollapseShowCommand;
        private VenueShow _expandedShow;
        private OrgEvent _orgEvent;

        public VenueViewModel(ISmartWalkDataService dataService, IExceptionPolicy exceptionPolicy)
        {
            _dataService = dataService;
            _exceptionPolicy = exceptionPolicy;
        }

        public event EventHandler RefreshCompleted;

        public override bool IsDescriptionExpandable
        {
            get
            {
                return Venue == null || (Venue.Shows != null && Venue.Shows.Length != 0);
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
                    RaisePropertyChanged(() => OrgEvent);

                    if (_orgEvent != null)
                    {
                        Venue = _orgEvent.Venues.FirstOrDefault(v => 
                            v.Number == _parameters.VenueNumber &&
                                string.Compare(v.Info.Name, _parameters.VenueName, true) == 0);
                    }
                    else
                    {
                        Venue = null;
                    }
                }
            }
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

                    if (Entity != null &&
                        ((Venue)Entity).Shows != null && 
                        ((Venue)Entity).Shows.Length == 1)
                    {
                        ExpandedShow = ((Venue)Entity).Shows.First();
                    }
                    else
                    {
                        ExpandedShow = null;
                    }
                }
            }
        }

        public VenueShow ExpandedShow
        {
            get
            {
                return _expandedShow;
            }
            private set
            {
                if (!Equals(_expandedShow, value))
                {
                    _expandedShow = value;
                    RaisePropertyChanged(() => ExpandedShow);
                }
            }
        }

        public ICommand RefreshCommand
        {
            get 
            {
                if (_refreshCommand == null)
                {
                    _refreshCommand = new MvxCommand(UpdateOrgEventAndVenue);
                }

                return _refreshCommand;
            }
        }

        public ICommand ExpandCollapseShowCommand
        {
            get
            {
                if (_expandCollapseShowCommand == null)
                {
                    _expandCollapseShowCommand = new MvxCommand<VenueShow>(show => 
                                                                           {
                        if (ExpandedShow != show)
                        {
                            ExpandedShow = show;
                        }
                        else 
                        {
                            ExpandedShow = null;
                        }
                    },
                    venue => _parameters != null);
                }

                return _expandCollapseShowCommand;
            }
        }

        public override bool CanShowNextEntity
        {
            get { return OrgEvent != null && OrgEvent.Venues.Length > 0 && Venue != null; }
        }

        public override bool CanShowPreviousEntity
        {
            get { return CanShowNextEntity; }
        }

        public void Init(Parameters parameters)
        {
            _parameters = parameters;

            UpdateOrgEventAndVenue();
        }

        protected override void OnShowPreviousEntity()
        {
            var currentIndex = Array.IndexOf(OrgEvent.Venues, Venue);
            if (currentIndex > 0)
            {
                Venue = OrgEvent.Venues[currentIndex - 1];
            }
            else
            {
                Venue = OrgEvent.Venues.Last();
            }
        }

        protected override void OnShowNextEntity()
        {
            var currentIndex = Array.IndexOf(OrgEvent.Venues, Venue);
            if (currentIndex < OrgEvent.Venues.Length - 1)
            {
                Venue = OrgEvent.Venues[currentIndex + 1];
            }
            else
            {
                Venue = OrgEvent.Venues.First();
            }
        }

        private void UpdateOrgEventAndVenue()
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
                                OrgEvent = orgEvent;

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