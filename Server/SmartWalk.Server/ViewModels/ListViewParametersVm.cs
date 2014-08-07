using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SmartWalk.Server.Utils;

namespace SmartWalk.Server.ViewModels
{
    public class ListViewParametersVm
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public SortType Sort { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public DisplayType Display { get; set; }
    }
}