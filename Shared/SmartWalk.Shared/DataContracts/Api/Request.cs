namespace SmartWalk.Shared.DataContracts.Api
{
    [UsedImplicitly]
    public class Request
    {
        public RequestSelect[] Selects { get; set; }

        public string[] Storages { get; set; }
    }
}