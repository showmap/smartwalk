using SmartWalk.Shared.Utils;

namespace SmartWalk.Shared.DataContracts.Api
{
    [UsedImplicitly]
    public class RequestSelectWhereSelectValue
    {
        public string SelectName { get; set; }

        public string Field { get; set; }

        public override int GetHashCode()
        {
            return 
                HashCode.Initial
                    .CombineHashCodeOrDefault(SelectName)
                    .CombineHashCodeOrDefault(Field);
        }
    }
}