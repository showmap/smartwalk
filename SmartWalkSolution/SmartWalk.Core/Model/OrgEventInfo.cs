using System;

namespace SmartWalk.Core.Model
{
    public class OrgEventInfo
    {
        public string OrgId { get; set; }

        public DateTime Date { get; set; }

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

        public bool HasSchedule { get; set; }
    }
}