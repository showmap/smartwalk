namespace SmartWalk.Shared.DataContracts.Protocol
{
    public class Request
    {
        public RequestSelect[] Selects { get; set; }

        public string[] Storages { get; set; }
    }
}