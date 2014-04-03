namespace SmartWalk.Shared.DataContracts.Api
{
    [UsedImplicitly]
    public class RequestSelectSortBy
    {
        public string Field { get; set; }

        public bool? IsDescending { get; set; }

        public double? OfDistance { get; set; }
    }
}
