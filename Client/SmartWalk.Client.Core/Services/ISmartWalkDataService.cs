using System;
using System.Threading.Tasks;
using SmartWalk.Client.Core.Model;

namespace SmartWalk.Client.Core.Services
{
    public interface ISmartWalkDataService
    {
        Task<LocationIndex> GetLocationIndex(DataSource source);

        Task<Org> GetOrg(string orgId, DataSource source);

        Task<OrgEvent> GetOrgEvent(string orgId, DateTime date, DataSource source);
    }
}