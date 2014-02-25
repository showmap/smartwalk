namespace SmartWalk.Shared.DataContracts.Protocol
{
    public class RequestSelectWhere
    {
        public string Field { get; set; }

        public string Operator { get; set; }

        public object Value { get; set; }
    }
}