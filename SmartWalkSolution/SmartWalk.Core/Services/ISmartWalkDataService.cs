using System;
using System.Collections.Generic;
using SmartWalk.Core.Model;

namespace SmartWalk.Core.Services
{
	public interface ISmartWalkDataService
	{
		void GetOrgInfos(Action<IEnumerable<EntityInfo>, Exception> resultHandler);

        void GetOrg(string orgId, Action<Org, Exception> resultHandler);

        void GetOrgEvent(OrgEventInfo eventInfo, Action<OrgEvent, Exception> resultHandler);
	}
}