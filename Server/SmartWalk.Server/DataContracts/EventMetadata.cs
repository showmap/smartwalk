using System;
using System.Runtime.Serialization;
using SmartWalk.Shared.DataContracts;

namespace SmartWalk.Server.DataContracts
{
    [DataContract]
    public class EventMetadata : IEventMetadata
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public IReference[] Host { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Title { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Description { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public DateTime? StartTime { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public DateTime? EndTime { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public CombineType? CombineType { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public IReference[] Shows { get; set; }
    }
}