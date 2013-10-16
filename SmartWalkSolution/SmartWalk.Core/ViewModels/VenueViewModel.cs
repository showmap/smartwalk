using System;
using System.Linq;
using System.Windows.Input;
using Cirrious.MvvmCross.Plugins.Email;
using Cirrious.MvvmCross.Plugins.PhoneCall;
using Cirrious.MvvmCross.ViewModels;
using SmartWalk.Core.Constants;
using SmartWalk.Core.Model;
using SmartWalk.Core.Services;

namespace SmartWalk.Core.ViewModels
{
    public class VenueViewModel : EntityViewModel
    {
        private readonly ISmartWalkDataService _dataService;
        private readonly IAnalyticsService _analyticsService;
        private readonly IExceptionPolicy _exceptionPolicy;

        private Parameters _parameters;
        private MvxCommand<VenueShow> _expandCollapseShowCommand;
        private VenueShow _expandedShow;
        private OrgEvent _orgEvent;

        public VenueViewModel(
            ISmartWalkDataService dataService, 
            IAnalyticsService analyticsService,
            IMvxPhoneCallTask phoneCallTask,
            IMvxComposeEmailTask composeEmailTask,
            IExceptionPolicy exceptionPolicy) : 
                base(analyticsService, phoneCallTask, composeEmailTask)
        {
            _dataService = dataService;
            _analyticsService = analyticsService;
            _exceptionPolicy = exceptionPolicy;
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

                    if (_orgEvent != null && _parameters != null)
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

                    if (Entity != null)
                    {
                        _parameters.VenueNumber = ((Venue)Entity).Number;
                        _parameters.VenueName = Entity.Info.Name;
                    }
                    else
                    {
                        _parameters.VenueNumber = 0;
                        _parameters.VenueName = null;
                    }

                    if (ExpandedShow != null && 
                        Entity != null &&
                        ((Venue)Entity).Shows != null &&
                        !((Venue)Entity).Shows.Contains(ExpandedShow))
                    {
                        ExpandedShow = null;
                    }

                    if (Entity != null &&
                        ((Venue)Entity).Shows != null && 
                        ((Venue)Entity).Shows.Length == 1)
                    {
                        ExpandedShow = ((Venue)Entity).Shows.First();
                    }

                    if (Entity != null &&
                        (((Venue)Entity).Shows == null || 
                        ((Venue)Entity).Shows.Length == 0))
                    {
                        IsDescriptionExpanded = true;
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

                            _analyticsService.SendEvent(
                                Analytics.CategoryUI,
                                Analytics.ActionTouch,
                                ExpandedShow != null 
                                    ? Analytics.ActionLabelExpandVenueShow 
                                    : Analytics.ActionLabelCollapseVenueShow,
                                GetExpandedShowNumber());
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

        protected override object InitParameters
        {
            get { return _parameters; }
        }

        public void Init(Parameters parameters)
        {
            _parameters = parameters;

            UpdateOrgEventAndVenue(DataSource.Cache);
        }

        protected override void Refresh()
        {
            UpdateOrgEventAndVenue(DataSource.Server);
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

            RaiseRefreshCompleted();
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

            RaiseRefreshCompleted();
        }

        private void UpdateOrgEventAndVenue(DataSource source)
        {
            if (_parameters != null)
            {
                _dataService.GetOrgEvent(
                    _parameters.OrgId,
                    _parameters.EventDate,
                    source,
                    (orgEvent, ex) => 
                        {
                            if (ex == null)
                            {
                                OrgEvent = orgEvent;
                            }
                            else
                            {
                                _exceptionPolicy.Trace(ex);
                            }
                    
                            RaiseRefreshCompleted();
                        });
            }
            else
            {
                Venue = null;
            }
        }

        private int GetExpandedShowNumber()
        {
            if (Venue != null && ExpandedShow != null)
            {
                return Array.IndexOf(Venue.Shows, ExpandedShow);
            }

            return 0;
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