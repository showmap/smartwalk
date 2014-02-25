namespace SmartWalk.Shared.DataContracts.Api
{
    public class RequestSelectWhere
    {
        public string Field { get; set; }

        public string Operator { get; set; }

        public dynamic Value { get; set; }
    }
}