using SmartWalk.Client.Core.Model;

namespace SmartWalk.Client.Core.Services
{
    public interface IShowDirectionsTask
    {
        void ShowDirections(AddressInfo addressInfo);
    }
}