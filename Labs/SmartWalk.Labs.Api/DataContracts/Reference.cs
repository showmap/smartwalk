using SmartWalk.Shared.DataContracts;

namespace SmartWalk.Labs.Api.DataContracts
{
    public class Reference : IReference
    {
        public int Id { get; set; }
        public string Storage { get; set; }
        public int? Type { get; set; }
    }
}
