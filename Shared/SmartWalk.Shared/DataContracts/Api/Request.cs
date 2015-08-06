using SmartWalk.Shared.Utils;

namespace SmartWalk.Shared.DataContracts.Api
{
    [UsedImplicitly]
    public class Request
    {
        public RequestSelect[] Selects { get; set; }

        public string[] Storages { get; set; }

        public string ClientVersion { get; set; }

        public override int GetHashCode()
        {
            return 
                HashCode.Initial
                    .CombineHashCodeOrDefault(Selects)
                    .CombineHashCodeOrDefault(Storages);
        }
    }
}