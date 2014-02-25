namespace SmartWalk.Server.Records
{
    public class StorageRecord
    {
        public virtual int Id { get; set; }
        public virtual string StorageKey { get; set; }
        public virtual string Description { get; set; }
    }

    public static class StorageKeys
    {
        public static readonly string[] All = new [] {
            SmartWalk,
            Facebook,
            Foursquare,
            GooglePlus,
            VKontakte
        };

        public const string SmartWalk = "SW";
        public const string Facebook = "FB";
        public const string Foursquare = "FS";
        public const string GooglePlus = "GP";
        public const string VKontakte = "VK";
    }
}