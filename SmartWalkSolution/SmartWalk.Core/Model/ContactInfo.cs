using System.Linq;
using SmartWalk.Core.Utils;
using SmartWalk.Core.Model.Interfaces;

namespace SmartWalk.Core.Model
{
    public class ContactInfo : ISearchable
    {
        public PhoneInfo[] Phones { get; set; }

        public EmailInfo[] Emails { get; set; }

        public WebSiteInfo[] WebSites { get; set; }

        public bool IsEmpty
        {
            get 
            {
                return (Phones == null || Phones.Length == 0) &&
                    (Emails == null || Emails.Length == 0) &&
                        (WebSites == null || WebSites.Length == 0);
            }
        }

        public string SearchableText
        {
            get
            {
                return (Phones != null  && Phones.Length != 0 ? " " + Phones.Select(p => p.SearchableText).Aggregate((a, b) => a + " " + b) : string.Empty) +
                    (Emails != null  && Emails.Length != 0 ? " " + Emails.Select(e => e.SearchableText).Aggregate((a, b) => a + " " + b) : string.Empty) +
                        (WebSites != null  && WebSites.Length != 0 ? " " + WebSites.Select(e => e.SearchableText).Aggregate((a, b) => a + " " + b) : string.Empty);
            }
        }

        public override bool Equals(object obj)
        {
            var info = obj as ContactInfo;
            if (info != null)
            {
                return Phones.EnumerableEquals(info.Phones) &&
                    Emails.EnumerableEquals(info.Emails)  &&
                        WebSites.EnumerableEquals(info.WebSites) ;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Initial
                .CombineHashCodeOrDefault(Phones)
                    .CombineHashCodeOrDefault(Emails)
                    .CombineHashCodeOrDefault(WebSites);
        }
    }
}