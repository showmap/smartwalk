using SmartWalk.Shared.Utils;
using SmartWalk.Client.Core.Model.Interfaces;

namespace SmartWalk.Client.Core.Model
{
    public class WebSiteInfo : ISearchable, IContact
    {
        public string Label { get; set; }

        public string URL { get; set; }

        public string Title
        {
            get
            {
                return Label;
            }
        }

        public string Contact
        {
            get
            {
                return URL;
            }
        }
        
        public ContactType Type
        {
            get
            {
                return ContactType.WebSite;
            } 
        }

        public string SearchableText
        {
            get
            {
                return (Label != null ? " " + Label : string.Empty) +
                    (URL != null ? " " + URL : string.Empty);
            }
        }

        public override bool Equals(object obj)
        {
            var info = obj as WebSiteInfo;
            if (info != null)
            {
                return Label == info.Label &&
                    URL == info.URL;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Initial
                .CombineHashCodeOrDefault(Label)
                    .CombineHashCodeOrDefault(URL);
        }
    }
}