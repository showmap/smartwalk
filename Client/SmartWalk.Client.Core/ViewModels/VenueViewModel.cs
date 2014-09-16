using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Cirrious.MvvmCross.ViewModels;
using SmartWalk.Client.Core.Constants;
using SmartWalk.Client.Core.Model;
using SmartWalk.Client.Core.Model.DataContracts;
using SmartWalk.Client.Core.Services;
using SmartWalk.Client.Core.Utils;
using SmartWalk.Client.Core.ViewModels.Common;
using SmartWalk.Shared.Utils;

namespace SmartWalk.Client.Core.ViewModels
{
    public class VenueViewModel : EntityViewModel
    {
        private readonly IEnvironmentService _environmentService;
        private readonly ISmartWalkApiService _apiService;
        private readonly IAnalyticsService _analyticsService;
        private readonly IExceptionPolicyService _exceptionPolicy;

        private Parameters _parameters;
        private MvxCommand  _copyAddressCommand;
        private MvxCommand<Show> _expandCollapseShowCommand;
        private Show _expandedShow;
        private Venue _venue;
        private Venue[] _orgEventVenues;
        private OrgEvent _orgEvent;

        public VenueViewModel(
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
            _environmentService = environmentService;
            _apiService = apiService;
            _analyticsService = analyticsService;
            _exceptionPolicy = exceptionPolicy;
        }

        public override string Title
        {
            get
            {
                if (Venue != null && Venue.Info != null)
                {
                    return Venue.Info.Name;
                }

                return null;
            }
        }

        public override string Subtitle
        {
            get
            {
                if (Venue != null && 
                    Venue.Info.HasAddresses())
                {
                    return Venue.Info.Addresses[0].AddressText;
                }

                return null;
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
                }
            }
        }

        public Venue[] OrgEventVenues
        {
            get
            {
                return _orgEventVenues;
            }
            private set
            {
                if (!Equals(_orgEventVenues, value))
                {
                    _orgEventVenues = value;
                    RaisePropertyChanged(() => OrgEventVenues);

                    if (_orgEventVenues != null && _parameters != null)
                    {
                        Venue = _orgEventVenues
                            .FirstOrDefault(v => v.Info.Id == _parameters.VenueId);
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
                return _venue;
            }
            private set
            {
                if (!Equals(_venue, value))
                {
                    _venue = value;
                    Entity = _venue != null ? _venue.Info : null;
                    RaisePropertyChanged(() => Venue);
                    RaisePropertyChanged(() => Title);

                    if (_venue != null)
                    {
                        _parameters.VenueId = _venue.Info.Id;
                    }
                    else
                    {
                        _parameters.VenueId = 0;
                    }

                    if (ExpandedShow != null && 
                        _venue != null &&
                        _venue.Shows != null &&
                        !_venue.Shows.Contains(ExpandedShow))
                    {
                        ExpandedShow = null;
                    }

                    if (_venue != null &&
                        _venue.Shows != null && 
                        _venue.Shows.Length == 1)
                    {
                        ExpandedShow = _venue.Shows[0];
                    }

                    /*if (_venue != null &&
                        _venue.Shows == null || 
                        _venue.Shows.Length == 0)
                    {
                        IsDescriptionExpanded = true;
                    }*/
                }
            }
        }

        public Show ExpandedShow
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
                    _expandCollapseShowCommand = new MvxCommand<Show>(show => 
                        {
                            ExpandedShow = ExpandedShow != show ? show : null;

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

        public ICommand CopyAddressCommand
        {
            get
            {
                if (_copyAddressCommand == null)
                {
                    _copyAddressCommand = new MvxCommand(() => 
                        {
                            _analyticsService.SendEvent(
                                Analytics.CategoryUI,
                                Analytics.ActionTouch,
                                Analytics.ActionLabelCopyAddress);

                            var address = Entity.Addresses
                                .Select(a => a.AddressText)
                                .FirstOrDefault();
                            _environmentService.Copy(address);
                        },
                        () => 
                            Entity != null &&
                            Entity.Addresses != null);
                }

                return _copyAddressCommand;
            }
        }

        public override bool CanShowNextEntity
        {
            get { return OrgEventVenues != null && OrgEventVenues.Length > 0 && Venue != null; }
        }

        public override string NextEntityTitle
        {
            get
            {
                var nextVenue = GetNextVenue();
                return nextVenue != null ? nextVenue.Info.Name : null;
            }
        }

        protected override ParametersBase InitParameters
        {
            get { return _parameters; }
        }

        public void Init(Parameters parameters)
        {
            _parameters = parameters;

            UpdateData(UpdateOrgEventVenues);
        }

        protected override void Refresh(DataSource source)
        {
            UpdateOrgEventVenues(source).ContinueWithThrow();
        }

        protected override void OnShowNextEntity()
        {
            Venue = GetNextVenue();
            RaiseRefreshCompleted(Venue != null);
        }

        private async Task<DataSource> UpdateOrgEventVenues(DataSource source, bool showProgress = true)
        {
            if (_parameters != null)
            {
                if (showProgress) IsLoading = true;

                var orgEvent = default(OrgEvent);
                var venues = default(Venue[]);

                try 
                {
                    var eventTask = _apiService.GetOrgEventInfo(
                        _parameters.EventId, source);
                    var venuesTask = _apiService.GetOrgEventVenues(
                        _parameters.EventId,
                        source);

                    await Task.WhenAll(eventTask, venuesTask);

                    if (eventTask.Result != null)
                    {
                        orgEvent = eventTask.Result.Data;
                    }

                    if (eventTask.Result != null)
                    {
                        venues = venuesTask.Result.Data;
                        source = venuesTask.Result.Source;
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

                if (venues != null)
                {
                    OrgEventVenues = venues;
                }

                if (showProgress)
                {
                    RaiseRefreshCompleted(OrgEventVenues != null);
                }
            }
            else
            {
                OrgEvent = null;
                Venue = null;
            }

            return source;
        }

        private int GetExpandedShowNumber()
        {
            if (Venue != null && ExpandedShow != null)
            {
                return Array.IndexOf(Venue.Shows, ExpandedShow);
            }

            return 0;
        }

        private Venue GetNextVenue()
        {
            var result = default(Venue);

            if (OrgEventVenues != null && Venue != null)
            {
                var currentIndex = Array.IndexOf(OrgEventVenues, Venue);
                if (currentIndex < OrgEventVenues.Length - 1)
                {
                    result = OrgEventVenues[currentIndex + 1];
                }
                else
                {
                    result = OrgEventVenues[0];
                }
            }

            return result;
        }

        public class Parameters : ParametersBase
        {
            public int VenueId { get; set; }
            public int EventId { get; set; }

            public override bool Equals(object obj)
            {
                var parameters = obj as Parameters;
                if (parameters != null)
                {
                    return Equals(Location, parameters.Location) && 
                        VenueId == parameters.VenueId &&
                        EventId == parameters.EventId;
                }

                return false;
            }

            public override int GetHashCode()
            {
                return HashCode.Initial
                    .CombineHashCodeOrDefault(Location)
                    .CombineHashCode(VenueId)
                    .CombineHashCode(EventId);
            }
        }
    }
}