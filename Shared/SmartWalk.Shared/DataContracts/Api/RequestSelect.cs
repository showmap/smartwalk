using SmartWalk.Shared.Utils;

namespace SmartWalk.Shared.DataContracts.Api
{
    [UsedImplicitly]
    public class RequestSelect
    {
        public string[] Fields { get; set; }
        public string As { get; set; }
        public string From { get; set; }
        public RequestSelectWhere[] Where { get; set; }
        public RequestSelectSortBy[] SortBy { get; set; }

        public override int GetHashCode()
        {
            return 
                HashCode.Initial
                    .CombineHashCodeOrDefault(Fields)
                    .CombineHashCodeOrDefault(As)
                    .CombineHashCodeOrDefault(From)
                    .CombineHashCodeOrDefault(Where)
                    .CombineHashCodeOrDefault(SortBy);
        }
    }
}