using System.Runtime.Serialization;

namespace SmartWalk.Shared.DataContracts.Api
{
    [DataContract]
    public class ResponseSelect
    {
        [DataMember(EmitDefaultValue = false)]
        public string Alias { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public object[] Records { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Error { get; set; }
    }
}
