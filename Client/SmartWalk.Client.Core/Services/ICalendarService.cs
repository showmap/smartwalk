using System.Threading.Tasks;
using SmartWalk.Client.Core.Model;

namespace SmartWalk.Client.Core.Services
{
    public interface ICalendarService
    {
        Task<CalendarEvent> CreateNewEvent(OrgEvent orgEvent);

        void SaveEvent(CalendarEvent calendarEvent);
    }
}