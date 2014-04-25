using System.Threading.Tasks;
using SmartWalk.Client.Core.Model;

namespace SmartWalk.Client.Core.Services
{
    public interface ISmartWalkApiService
    {
        Task<OrgEvent[]> GetOrgEvents(Location location, DataSource source);

        Task<OrgEvent> GetOrgEvent(int id, DataSource source);

        Task<OrgEvent> GetOrgEventInfo(int id, DataSource source);

        Task<Venue[]> GetOrgEventVenues(int id, DataSource source);

        Task<Org> GetHost(int id, DataSource source);
    }
}