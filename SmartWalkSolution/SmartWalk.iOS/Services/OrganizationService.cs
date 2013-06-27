using System;
using SmartWalk.Core.Services;
using System.Collections;
using System.Collections.Generic;
using SmartWalk.Core;

namespace SmartWalk.iOS.Services
{
    public class OrganizationService : IOrganizationService
    {
        public OrganizationService()
        {
        }

        public void GetOrganizations(Action<IEnumerable<Organization>, Exception> resultHandler)
        {
            resultHandler(new List<Organization> 
                {
                    new Organization { Name = "test" },
                    new Organization { Name = "test2" }
                }, null);
        }
    }
}