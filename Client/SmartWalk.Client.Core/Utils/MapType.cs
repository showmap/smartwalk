namespace SmartWalk.Client.Core.Utils
{
    public enum MapType
    {
        Standard,
        Satellite,
        Hybrid
    }

    public static class MapTypeExtensions
    {
        public static MapType GetNextMapType(this MapType mapType)
        {
            switch (mapType)
            {
                case MapType.Standard:
                    return MapType.Satellite;

                case MapType.Satellite:
                    return MapType.Hybrid;

                case MapType.Hybrid:
                    return MapType.Standard;
            }

            return MapType.Standard;
        }
    }
}