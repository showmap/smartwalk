using SmartWalk.Core.Utils;

namespace SmartWalk.Core.Model
{
    public class WebSiteInfo : ISearchable
    {
        public string Label { get; set; }

        public string URL { get; set; }

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