using System.Linq;
using SmartWalk.Client.Core.Utils;
using SmartWalk.Client.Core.Model.Interfaces;

namespace SmartWalk.Client.Core.Model
{
    public class Venue : Entity, INumberEntity
    {
        public int Number { get; set; }

        public VenueShow[] Shows { get; set; }

        public override string SearchableText
        {
            get
            {
                return (Number != 0 ? " " + Number : string.Empty) + 
                    base.SearchableText;
            }
        }

        public Venue Clone()
        {
            return new Venue {
                Number = Number,
                Info = Info != null ? Info.Clone() : null,
                Description = Description,
                Shows = Shows != null ? Shows.Select(s => s.Clone()).ToArray() : null
            };
        }

        public override bool Equals(object obj)
        {
            var venue = obj as Venue;
            if (venue != null)
            {
                return Number == venue.Number && 
                    Equals(Info, venue.Info) &&
                        Description == venue.Description &&
                            Shows.EnumerableEquals(venue.Shows);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Initial
                .CombineHashCode(Number)
                    .CombineHashCodeOrDefault(Info)
                        .CombineHashCodeOrDefault(Description)
                            .CombineHashCodeOrDefault(Shows);
        }
    }
}