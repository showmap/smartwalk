using SmartWalk.Shared.Utils;
using SmartWalk.Labs.Api.DataContracts;

namespace SmartWalk.Labs.Api.Model
{
    public class Venue
    {
        public const int DayGroupId = -1000;

        public Venue(Entity entity, string description)
        {
            Info = entity;
            Description = description;
        }

        public int? Number { get; set; }
        public Entity Info { get; private set; }
        public string Description { get; private set; }
        public Show[] Shows { get; set; }

        public override bool Equals(object obj)
        {
            var venue = obj as Venue;
            if (venue != null)
            {
                var result = Equals(Info, venue.Info) &&
                    Shows.EnumerableEquals(venue.Shows) &&
                    Description == venue.Description;
                return result;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Initial
                .CombineHashCode(Info.GetHashCode())
                .CombineHashCodeOrDefault(Shows)
                .CombineHashCodeOrDefault(Description);
        }

        public override string ToString()
        {
            return string.Format(
                "Id={0}, ShowsCount={1}", 
                Info.Id, 
                Shows != null ? Shows.Length : 0);
        }
    }
}