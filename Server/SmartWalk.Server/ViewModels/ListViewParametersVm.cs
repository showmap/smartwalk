using System;
using SmartWalk.Server.Extensions;

namespace SmartWalk.Server.ViewModels
{
    public class ListViewParametersVm
    {
        private DisplayType _display = DisplayType.My;

        public ListViewParametersVm()
        {
            Sort = SortType.Date;
            IsLoggedIn = false;
        }

        public SortType Sort { get; set; }

        public bool IsLoggedIn { get; set; }

        public DisplayType Display
        {
            get { return IsLoggedIn ? _display : DisplayType.All; }
            set { _display = value; }
        }

        public string Url
        {
            get
            {
                switch (Display)
                {
                    case DisplayType.All:
                        return "";

                    case DisplayType.My:
                        return "/" + DisplayType.My.ToString().ToLower();
                }

                return "";
            }
        }

        public void LoadParameters(bool isLoggedIn, string sort)
        {
            IsLoggedIn = isLoggedIn;
            Sort =
                String.IsNullOrEmpty(sort)
                    ? SortType.Date
                    : (SortType)Enum.Parse(typeof(SortType), sort.ToUpperFirstLetter());
        }
    }

    public enum SortType
    {
        Title,
        Date
    }

    public enum DisplayType
    {
        All,
        My
    }
}