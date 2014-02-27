namespace SmartWalk.Shared.DataContracts.Api
{
    public class RequestSelectWhere
    {
        public string Field { get; set; }

        public string Operator { get; set; }

        public object Value { get; set; }

        public object[] Values { get; set; }

        public RequestSelectWhereSelectValue SelectValue { get; set; }
    }
}