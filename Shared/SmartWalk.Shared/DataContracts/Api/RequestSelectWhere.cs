namespace SmartWalk.Shared.DataContracts.Api
{
    public class RequestSelectWhere
    {
        public string Field { get; set; }

        public string Operator { get; set; }

        public string Value { get; set; }

        public string[] Values { get; set; }

        public RequestSelectWhereSelectValue SelectValue { get; set; }
    }
}