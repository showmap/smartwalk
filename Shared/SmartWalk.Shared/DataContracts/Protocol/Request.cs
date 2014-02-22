namespace SmartWalk.Shared.DataContracts.Protocol
{
    public class Request
    {
        public RequestSelect[] Selects { get; set; }

        public Storage[] Storages { get; set; }
    }
}