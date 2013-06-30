using SmartWalk.Core.Model;
using Cirrious.MvvmCross.ViewModels;
using SmartWalk.Core.Services;

namespace SmartWalk.Core.ViewModels
{
    public class EventViewModel : MvxViewModel
    {
        private readonly ISmartWalkDataService _dataService;

        private OrgEventInfo _eventInfo;
        private OrgEvent _orgEvent;

        public EventViewModel(ISmartWalkDataService dataService)
        {
            _dataService = dataService;
        }

        public OrgEventInfo EventInfo
        {
            get
            {
                return _eventInfo;
            }
            set
            {
                if (!Equals(_eventInfo, value))
                {
                    _eventInfo = value;
                    RaisePropertyChanged(() => EventInfo);
                }
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

        private void UpdateEvent()
        {
            if (EventInfo != null)
            {
                _dataService.GetOrgEvent(EventInfo, (orgEvent, ex) => 
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
    }
}