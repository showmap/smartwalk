using System.Collections.Generic;

namespace SmartWalk.Core.Model
{
    public class ContactInfo
    {
        public IEnumerable<ContactPhoneInfo> Phones { get; set; }

        public IEnumerable<ContactEmailInfo> Emails { get; set; }

        public string Web { get; set; }

        public string Facebook { get; set; }
    }
}