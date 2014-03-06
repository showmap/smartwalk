using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Orchard.Environment.Extensions;
using Orchard.Mvc.AntiForgery;
using Orchard.Mvc.Filters;
using Orchard.Security;
using Orchard.Settings;

namespace SmartWalk.Server.Extensions
{
    [OrchardSuppressDependency("Orchard.Mvc.AntiForgery.AntiForgeryAuthorizationFilter")]
    public class AntiForgeryRelocation : FilterProvider, IAuthorizationFilter
    {
        private readonly ISiteService _siteService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IExtensionManager _extensionManager;

        private readonly AntiForgeryAuthorizationFilter _orgFilter;


        public AntiForgeryRelocation(ISiteService siteService, IAuthenticationService authenticationService, IExtensionManager extensionManager)
        {
            _siteService = siteService;
            _authenticationService = authenticationService;
            _extensionManager = extensionManager;

            _orgFilter = new AntiForgeryAuthorizationFilter(siteService, authenticationService, extensionManager);
        }

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }

            const string tokenFieldName = "X-Request-Verification-Token";
            var httpContext = filterContext.HttpContext;
            var cookie = httpContext.Request.Cookies[AntiForgeryConfig.CookieName];
            var headerToken = httpContext.Request.Headers[tokenFieldName];

            if (!string.IsNullOrEmpty(headerToken))
                AntiForgery.Validate(cookie != null ? cookie.Value : null, headerToken);
            else
                _orgFilter.OnAuthorization(filterContext);
        }
    }
}