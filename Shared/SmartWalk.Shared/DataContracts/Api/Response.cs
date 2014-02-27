using System.Runtime.Serialization;

namespace SmartWalk.Shared.DataContracts.Api
{
    [DataContract]
    public class Response
    {
        public ResponseSelect[] Selects { get; set; }
    }
}