using SmartWalk.Core.Utils;

namespace SmartWalk.Core.Model
{
    public class ContactInfo
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