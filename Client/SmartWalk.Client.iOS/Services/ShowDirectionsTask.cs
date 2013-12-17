using SmartWalk.Core.Model;
using SmartWalk.Core.Services;
using SmartWalk.iOS.Utils.Map;

namespace SmartWalk.iOS.Services
{
    public class ShowDirectionsTask : IShowDirectionsTask
    {
        public void ShowDirections(AddressInfo addressInfo)
        {
            MapUtil.OpenAddressInMaps(addressInfo);
        }
    }
}