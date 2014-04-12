using SmartWalk.Client.Core.Model.DataContracts;
using SmartWalk.Client.Core.Services;
using SmartWalk.Client.iOS.Utils.Map;

namespace SmartWalk.Client.iOS.Services
{
    public class ShowDirectionsTask : IShowDirectionsTask
    {
        public void ShowDirections(Address address)
        {
            MapUtil.OpenAddressInMaps(address);
        }
    }
}