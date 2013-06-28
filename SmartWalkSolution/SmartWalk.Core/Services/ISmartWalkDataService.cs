using System;
using System.Collections.Generic;
using SmartWalk.Core.Model;

namespace SmartWalk.Core.Services
{
	public interface ISmartWalkDataService
	{
		void GetOrgs(Action<IEnumerable<Organization>, Exception> resultHandler);

        void GetOrgEvents(string orgId, Action<IEnumerable<OrgEvent>, Exception> resultHandler);
	}
}