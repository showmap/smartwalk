using System;
using SmartWalk.Core.Model;

namespace SmartWalk.Core.Services
{
	public interface ISmartWalkDataService
	{
        void GetLocation(Action<Location, Exception> resultHandler);

        void GetOrg(string orgId, Action<Org, Exception> resultHandler);

        void GetOrgEvent(
            string orgId, 
            DateTime date, 
            DataSource source, 
            Action<OrgEvent, Exception> resultHandler);
	}
}