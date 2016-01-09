namespace SmartWalk.Labs.Api
{
    public class MainClass
    {
#if LOCAL
        private const string SmartWalkUrl = "http://localhost/api";
#else
		private const string SmartWalkUrl = "http://showmap.co/api";
#endif

        public static void Main(string[] args)
        {
			SerializeChecks.Run(SmartWalkUrl);
			//Downloader.Run(SmartWalkUrl);
        }
    }
}