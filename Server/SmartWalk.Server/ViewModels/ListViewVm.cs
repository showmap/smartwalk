using System.Collections.Generic;

namespace SmartWalk.Server.ViewModels
{
    public class ListViewVm<T>
    {
        public ListViewParametersVm Parameters { get; set; }
        public IList<T> Data { get; set; }
    }
}