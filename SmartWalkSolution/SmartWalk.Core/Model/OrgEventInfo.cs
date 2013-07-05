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

        public int TimeStatus
        {
            get 
            {
                if (Date < DateTime.Now.AddDays(-2))
                {
                    return -1;
                }
                else if (DateTime.Now.AddDays(-2) <= Date && Date <= DateTime.Now.AddDays(2))
                {
                    return 0;
                }
                else if (Date > DateTime.Now.AddDays(2))
                {
                    return 1;
                }

                return -2;
            }
        }
    }
}