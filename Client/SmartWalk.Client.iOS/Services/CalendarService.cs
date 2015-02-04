using System;
using System.Threading.Tasks;
using EventKit;
using Foundation;
using UIKit;
using SmartWalk.Client.Core.Model;
using SmartWalk.Client.Core.Resources;
using SmartWalk.Client.Core.Services;
using SmartWalk.Client.iOS.Utils;

namespace SmartWalk.Client.iOS.Services
{
    public class CalendarService : ICalendarService
    {
        private readonly IConfiguration _configuration;

        private EKEventStore _eventStore;

        public CalendarService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private EKEventStore EventStore
        {
            get 
            {
                if (_eventStore == null)
                {
                    _eventStore = new EKEventStore();
                }

                return _eventStore;
            }
        }

        public async Task<CalendarEvent> CreateNewEvent(OrgEvent orgEvent)
        {
            var result = await EventStore.RequestAccessAsync(EKEntityType.Event);
            if (result.Item1 && result.Item2 == null)
            {
                var newEvent = EKEvent.FromStore(EventStore);

                newEvent.AllDay = true;
                newEvent.StartDate = orgEvent.StartTime.ToNSDate();
                newEvent.EndDate = (orgEvent.EndTime ?? orgEvent.StartTime).ToNSDate();
                newEvent.Location = string.Format("{0},{1}", orgEvent.Latitude, orgEvent.Longitude);
                newEvent.Title = orgEvent.Title;
                newEvent.Notes = orgEvent.Description;
                newEvent.Url = _configuration.GetEventUrl(orgEvent.Id).ToNSUrl();

                newEvent.Calendar = EventStore.DefaultCalendarForNewEvents;

                var calendarEvent = new CalendarEvent(EventStore, newEvent);
                return calendarEvent;
            }

            new UIAlertView(
                Localization.AccessDenied, 
                Localization.UserDeniedAccessToCalendars, 
                null, 
                Localization.OK, 
                null).Show();

            return null;
        }

        public void SaveEvent(CalendarEvent calendarEvent)
        {
            NSError error;
            EventStore.SaveEvent((EKEvent)calendarEvent.EventObj, EKSpan.ThisEvent, out error);

            if (error != null)
            {
                throw new NSErrorException(error);
            }
        }
    }
}