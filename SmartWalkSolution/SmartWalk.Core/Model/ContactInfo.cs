namespace SmartWalk.Core.Model
{
    public class ContactInfo
    {
        public ContactPhoneInfo[] Phones { get; set; }

        public ContactEmailInfo[] Emails { get; set; }

        public string Web { get; set; }

        public string Facebook { get; set; }
    }
}