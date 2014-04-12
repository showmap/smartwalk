namespace SmartWalk.Client.Core.Model
{
    // Analysis disable CompareOfFloatsByEqualityOperator
    public struct Location 
    {
        public static readonly Location Empty = new Location(0, 0);

        public Location(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        public readonly double Latitude;
        public readonly double Longitude;

        public static bool operator ==(Location l1, Location l2) 
        {
            return l1.Latitude == l2.Latitude && 
                l1.Longitude == l2.Longitude;
        }

        public static bool operator !=(Location l1, Location l2) 
        {
            return l1.Latitude != l2.Latitude || 
                l1.Longitude != l2.Longitude;
        }

        public override bool Equals(object obj)
        {
            return obj is Location && this == (Location)obj;
        }

        public override int GetHashCode()
        {
            return (int)Latitude ^ (int)Longitude;
        }

        public override string ToString()
        {
            return string.Format("({0},{1})", Latitude, Longitude);
        }
    }
}