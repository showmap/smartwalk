using System.Threading.Tasks;

namespace SmartWalk.Client.Core.Services
{
    public interface IHttpService
    {
        Task<TResponse> Get<TResponse>(object request) where TResponse : class;
    }
}