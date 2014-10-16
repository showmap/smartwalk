using SmartWalk.Shared.Utils;
using SmartWalk.Client.Core.Model.DataContracts;

namespace SmartWalk.Client.Core.Model
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
                return Equals(Info, venue.Info) &&
                    Shows.EnumerableEquals(venue.Shows) &&
                    Description == venue.Description;
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