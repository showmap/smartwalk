using SmartWalk.Shared.Utils;

namespace SmartWalk.Shared.DataContracts.Api
{
    [UsedImplicitly]
    public class RequestSelect
    {
        public int? Offset { get; set; }
        public int? Fetch { get; set; }
        public string[] Fields { get; set; }
        public string As { get; set; }
        public string From { get; set; }
        public RequestSelectWhere[] Where { get; set; }
        public RequestSelectSortBy[] SortBy { get; set; }

        public PictureSize? PictureSize { get; set; }

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