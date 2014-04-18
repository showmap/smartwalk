using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartWalk.Server.ViewModels
{
    public class ListViewParametersVm
    {
        public SortType Sort { get; set; }
        private DisplayType _display;
        public DisplayType Display {
            get { return IsLoggedIn ? _display : DisplayType.All; }
            set { _display = value; }
        }
        public bool IsLoggedIn { get; set; }

        public ListViewParametersVm() {
            Sort = SortType.Date;
            _display = DisplayType.My;
            IsLoggedIn = false;
        }
    }

    public enum SortType {
        Title,
        Date
    }

    public enum DisplayType {
        All,
        My
    }
}