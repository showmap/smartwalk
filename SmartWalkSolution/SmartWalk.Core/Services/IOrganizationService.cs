using System;
using System.Collections.Generic;

namespace SmartWalk.Core.Services
{
	public interface IOrganizationService
	{
		void GetOrganizations(Action<IEnumerable<Organization>, Exception> resultHandler);
	}
}

