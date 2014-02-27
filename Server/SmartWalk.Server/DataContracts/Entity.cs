using System.Runtime.Serialization;
using SmartWalk.Shared.DataContracts;

namespace SmartWalk.Server.DataContracts
{
    [DataContract]
    public class Entity : IEntity
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public EntityType? Type { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Name { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Description { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Picture { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public IContact[] Contacts { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public IAddress[] Addresses { get; set; }
    }
}