using System.Threading.Tasks;
using SmartWalk.Client.Core.Model;

namespace SmartWalk.Client.Core.Services
{
    public interface ISmartWalkApiService
    {
        Task<IApiResult<OrgEvent[]>> GetOrgEvents(Location location, DataSource source);

        Task<IApiResult<OrgEvent>> GetOrgEvent(int id, DataSource source);

        Task<IApiResult<OrgEvent>> GetOrgEventInfo(int id, DataSource source);

        Task<IApiResult<Venue[]>> GetOrgEventVenues(int id, DataSource source);

        Task<IApiResult<Org>> GetHost(int id, DataSource source);
    }

    public enum DataSource
    {
        Cache,
        CacheOrServer,
        Server
    }

    public interface IApiResult<T>
    {
        T Data { get; }
        DataSource Source { get; }
    }

    public class ApiResult<T> : IApiResult<T> 
    {
        public T Data { get; set; }
        public DataSource Source { get; set; }
    }
}