using System;
using SmartWalk.Core.Model;

namespace SmartWalk.Core.Services
{
	public interface ISmartWalkDataService
	{
        void GetLocationIndex(
            DataSource source,
            Action<LocationIndex, Exception> resultHandler);

        void GetOrg(
            string orgId,
            DataSource source,
            Action<Org, Exception> resultHandler);

        void GetOrgEvent(
            string orgId, 
            DateTime date, 
            DataSource source, 
            Action<OrgEvent, Exception> resultHandler);
	}
}