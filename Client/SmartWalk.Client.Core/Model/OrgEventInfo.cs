using System;
using SmartWalk.Shared.Utils;

namespace SmartWalk.Client.Core.Model
{
    public class OrgEventInfo
    {
        public string OrgId { get; set; }

        public DateTime Date { get; set; }

        public bool HasSchedule { get; set; }

        public int TimeStatus
        {
            get 
            {
                if (Date < DateTime.Now.AddDays(-2))
                {
                    return -1;
                }
                else if (DateTime.Now.AddDays(-2) <= Date && 
                    Date <= DateTime.Now.AddDays(2))
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

        public override bool Equals(object obj)
        {
            var eventInfo = obj as OrgEventInfo;
            if (eventInfo != null)
            {
                return OrgId == eventInfo.OrgId &&
                    Date == eventInfo.Date &&
                        HasSchedule == eventInfo.HasSchedule;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Initial
                .CombineHashCode(OrgId)
                    .CombineHashCode(Date)
                        .CombineHashCode(HasSchedule);
        }
    }
}