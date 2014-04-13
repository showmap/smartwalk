using SmartWalk.Shared.Utils;

namespace SmartWalk.Shared.DataContracts.Api
{
    [UsedImplicitly]
    public class RequestSelectSortBy
    {
        public string Field { get; set; }

        public bool? IsDescending { get; set; }

        public double? OfDistance { get; set; }

        public override int GetHashCode()
        {
            return 
                HashCode.Initial
                    .CombineHashCodeOrDefault(Field)
                    .CombineHashCode(IsDescending)
                    .CombineHashCode(OfDistance);
        }
    }
}
