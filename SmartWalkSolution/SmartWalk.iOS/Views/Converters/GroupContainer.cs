using System;
using System.Collections.Generic;

namespace SmartWalk.iOS.Views.Converters
{
    public class GroupContainer : List<object>
    {
        public GroupContainer(IEnumerable<object> items)
        {
            this.AddRange(items);
        }

        public string Key { get; set; }
    }
}