using SmartWalk.Shared.Utils;
using SmartWalk.Client.Core.Model.DataContracts;

namespace SmartWalk.Client.Core.Model
{
    public class Venue
    {
        private readonly Entity _entity;

        public Venue(Entity entity)
        {
            _entity = entity;
        }

        public Entity Info
        {
            get { return _entity; }
        }

        public Show[] Shows { get; set; }

        public override bool Equals(object obj)
        {
            var venue = obj as Venue;
            if (venue != null)
            {
                return Info.Id == venue.Info.Id;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Initial
                    .CombineHashCode(_entity.GetHashCode())
                    .CombineHashCodeOrDefault(Shows);
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