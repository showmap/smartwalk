using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using Orchard;

namespace SmartWalk.Server.Services.CultureService
{
    public interface ICultureService : IDependency {
        CultureInfo GetCurrentCulture();
    }

}