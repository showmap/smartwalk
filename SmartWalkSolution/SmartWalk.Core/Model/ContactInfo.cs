namespace SmartWalk.Core.Model
{
    public class ContactInfo
    {
        public ContactPhoneInfo[] Phones { get; set; }

        public ContactEmailInfo[] Emails { get; set; }

        public ContactWebSiteInfo[] WebSites { get; set; }

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