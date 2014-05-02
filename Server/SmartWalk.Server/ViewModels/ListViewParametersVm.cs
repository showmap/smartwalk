using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SmartWalk.Server.Extensions;

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
        public string Url {
            get {
                switch (Display) {
                    case DisplayType.All:
                        return "";
                    case DisplayType.My:
                        return "/" + DisplayType.My.ToString().ToLower();
                }

                return "";
            }
        }


        public ListViewParametersVm() {
            Sort = SortType.Date;
            _display = DisplayType.My;
            IsLoggedIn = false;
        }

        public void LoadParameters(bool isLoggedIn, string sort) {
            IsLoggedIn = isLoggedIn;
            Sort = String.IsNullOrEmpty(sort) ? SortType.Date : (SortType)Enum.Parse(typeof (SortType), sort.ToUpperFirstLetter());
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