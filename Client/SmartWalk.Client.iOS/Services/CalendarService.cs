using System;
using System.Threading.Tasks;
using MonoTouch.EventKit;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Client.Core.Model;
using SmartWalk.Client.Core.Services;
using SmartWalk.Client.iOS.Resources;

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
            var hasAccess = await EventStore.RequestAccessAsync(EKEntityType.Event);
            if (hasAccess)
            {
                var newEvent = EKEvent.FromStore(EventStore);

                newEvent.AllDay = true;
                newEvent.StartDate = orgEvent.StartTime;
                newEvent.EndDate = orgEvent.EndTime ?? orgEvent.StartTime;
                newEvent.Location = string.Format("{0},{1}", orgEvent.Latitude, orgEvent.Longitude);
                newEvent.Title = orgEvent.Title;
                newEvent.Notes = orgEvent.Description;
                newEvent.Url = new NSUrl(_configuration.GetEventUrl(orgEvent.Id));

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