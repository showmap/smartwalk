using Orchard;
using SmartWalk.Shared.DataContracts.Api;

namespace SmartWalk.Server.Services.QueryService
{
    public interface IQueryService : IDependency 
    {
        Response ExecuteRequestQuery(Request request);
    }
}