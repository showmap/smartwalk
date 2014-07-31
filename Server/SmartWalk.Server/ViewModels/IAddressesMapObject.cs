using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartWalk.Server.ViewModels
{
    public interface IAddressesMapObject
    {
        double Latitude { get; }
        double Longitude { get; }
        IList<AddressVm> Addresses { get; }
    }
}