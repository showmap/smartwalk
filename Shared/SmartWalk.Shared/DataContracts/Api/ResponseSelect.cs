namespace SmartWalk.Shared.DataContracts.Api
{
    [UsedImplicitly]
    public class ResponseSelect
    {
        public string Alias { get; set; }

        public object[] Records { get; set; }

        public string Error { get; set; }
    }
}