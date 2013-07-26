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
    }
}