using SmartWalk.Shared.Utils;

namespace SmartWalk.Shared.DataContracts.Api
{
    [UsedImplicitly]
    public class RequestSelectWhere
    {
        public string Field { get; set; }

        public string Operator { get; set; }

        public object Value { get; set; }

        public object[] Values { get; set; }

        public RequestSelectWhereSelectValue SelectValue { get; set; }

        public override int GetHashCode()
        {
            return 
                HashCode.Initial
                    .CombineHashCodeOrDefault(Field)
                    .CombineHashCodeOrDefault(Operator)
                    .CombineHashCodeOrDefault(Value)
                    .CombineHashCodeOrDefault(Values)
                    .CombineHashCodeOrDefault(SelectValue);
        }
    }
}