using System.Linq;
using SmartWalk.Core.Model.Interfaces;

namespace SmartWalk.Core.Model
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
    }
}