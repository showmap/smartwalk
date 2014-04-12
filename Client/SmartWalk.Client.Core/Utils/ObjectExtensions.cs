using System;
using Newtonsoft.Json;

namespace SmartWalk.Client.Core.Utils
{
    public static class ObjectExtensions
    {
        public static T Clone<T>(this object obj)
        {
            var text = JsonConvert.SerializeObject(obj);
            return JsonConvert.DeserializeObject<T>(text);
        }
    }
}