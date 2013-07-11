using System;
using SmartWalk.Core.Model;

namespace SmartWalk.Core.Services
{
	public interface ISmartWalkDataService
	{
		void GetOrgInfos(Action<EntityInfo[], Exception> resultHandler);

        void GetOrg(string orgId, Action<Org, Exception> resultHandler);

        void GetOrgEvent(string orgId, DateTime date, Action<OrgEvent, Exception> resultHandler);
	}
}