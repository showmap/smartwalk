using SmartWalk.Shared.Utils;

namespace SmartWalk.Client.Core.Model
{
    public class CalendarEvent
    {
        public CalendarEvent(object eventStore, object eventObj)
        {
            EventStore = eventStore;
            EventObj = eventObj;
        }

        public object EventStore { get; private set; }
        public object EventObj { get; private set; }

        public override bool Equals(object obj)
        {
            var calendarEvent = obj as CalendarEvent;
            if (calendarEvent != null)
            {
                return Equals(EventStore, calendarEvent.EventStore) &&
                    Equals(EventObj, calendarEvent.EventObj);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Initial
                    .CombineHashCodeOrDefault(EventStore)
                    .CombineHashCodeOrDefault(EventObj);
        }
    }
}