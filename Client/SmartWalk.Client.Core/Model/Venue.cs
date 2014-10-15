using SmartWalk.Shared.Utils;
using SmartWalk.Client.Core.Model.DataContracts;

namespace SmartWalk.Client.Core.Model
{
    public class Venue
    {
        public const int DayGroupId = -1000;

        private readonly Entity _entity;

        public Venue(Entity entity)
        {
            _entity = entity;
        }

        public Entity Info
        {
            get { return _entity; }
        }

        public int? EventSortOrder { get; set; }
        public string EventDescription { get; set; }
        public Show[] Shows { get; set; }

        public override bool Equals(object obj)
        {
            var venue = obj as Venue;
            if (venue != null)
            {
                return Equals(_entity, venue._entity) &&
                    Shows.EnumerableEquals(venue.Shows) &&
                    EventSortOrder == venue.EventSortOrder &&
                    EventDescription == venue.EventDescription;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Initial
                    .CombineHashCode(_entity.GetHashCode())
                    .CombineHashCodeOrDefault(Shows)
                    .CombineHashCode(EventSortOrder)
                    .CombineHashCodeOrDefault(EventDescription);
        }

        public override string ToString()
        {
            return string.Format(
                "Id={0}, ShowsCount={1}, Order={2}", 
                Info.Id, 
                Shows != null ? Shows.Length : 0,
                EventSortOrder ?? 0);
        }
    }
}