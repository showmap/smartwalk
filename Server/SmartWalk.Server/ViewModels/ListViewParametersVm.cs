using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SmartWalk.Shared;

namespace SmartWalk.Server.ViewModels
{
    public class ListViewParametersVm
    {
        [UsedImplicitly]
        [JsonConverter(typeof(StringEnumConverter))]
        public SortType Sort { get; set; }

        [UsedImplicitly]
        [JsonConverter(typeof(StringEnumConverter))]
        public DisplayType Display { get; set; }
    }

    public enum SortType
    {
        None = 0,
        Title = 1,
        Date = 2,
    }

    public enum DisplayType
    {
        All = 0,
        My = 1
    }
}