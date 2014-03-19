using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using Orchard;
using Orchard.Localization.Services;

namespace SmartWalk.Server.Services.CultureService
{
    public class CultureService : ICultureService
    {
        private readonly ICultureManager _cultureManager;
        private readonly IOrchardServices _orchardServices;

        private const string DefaultCulture = "en-US";

        public CultureService(ICultureManager cultureManager, IOrchardServices orchardServices) {
            _cultureManager = cultureManager;
            _orchardServices = orchardServices;
        }

        public CultureInfo GetCurrentCulture() {
            if (_orchardServices.WorkContext.HttpContext == null)
                return CultureInfo.GetCultureInfo(DefaultCulture);

            return CultureInfo.GetCultureInfo(_cultureManager.GetCurrentCulture(_orchardServices.WorkContext.HttpContext));
        }
    }
}