namespace SmartWalk.Shared.DataContracts.Protocol
{
    public class RequestSelectWhere
    {
        public string Field { get; set; }

        public WhereOperator Operator { get; set; }

        public SelectValue SelectValue { get; set; } 

        public string Value { get; set; }

        public string[] Values { get; set; }
    }

    public enum WhereOperator
    {
        Equals,
        ContainsIn
    }

    public class SelectValue
    {
        public string SelectName { get; set; }

        public string Field { get; set; }
    }
}