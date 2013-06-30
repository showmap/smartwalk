using System;

namespace SmartWalk.Core.Model
{
    public class OrgEventInfo
    {
        public string OrgId { get; set; }

        public DateTime Date { get; set; }

        public string Day
        {
            get
            {
                return Date != DateTime.MinValue ? String.Format("{0:dd}", Date) : null;
            }
        }

        public string Month
        {
            get
            {
                return Date != DateTime.MinValue ? String.Format("{0:M}", Date) : null;
            }
        }
    }
}