namespace SmartWalk.Core.Model
{
    public class Venue : Entity
    {
        public int Number { get; set; }

        public VenueShow[] Shows { get; set; }
    }
}