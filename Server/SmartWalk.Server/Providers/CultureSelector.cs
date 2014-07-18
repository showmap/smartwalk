using System.Globalization;
using System.Linq;
using System.Web;
using Orchard;
using Orchard.Localization.Services;
using SmartWalk.Shared;

namespace SmartWalk.Server.Providers
{
    [UsedImplicitly]
    public class CultureSelector : ICultureSelector
    {
        private readonly IWorkContextAccessor _workContextAccessor;

        public CultureSelector(IWorkContextAccessor workContextAccessor)
        {
            _workContextAccessor = workContextAccessor;
        }

        public CultureSelectorResult GetCulture(HttpContextBase context)
        {
            var workContext = _workContextAccessor.GetContext();
            if (workContext == null ||
                workContext.HttpContext == null ||
                workContext.HttpContext.Request == null ||
                workContext.HttpContext.Request.UserLanguages == null) return null;

            var browserCultures =
                workContext.HttpContext.Request.UserLanguages
                           .Select(ul => ul.Split(';').FirstOrDefault())
                           .Where(ul => !string.IsNullOrWhiteSpace(ul))
                           .ToArray();
            if (browserCultures.Length == 0) return null;

            var cultureName =
                browserCultures
                    .Select(ParseCultureInfo)
                    .Where(ci => ci != null)
                    .Select(ci => ci.Name)
                    .FirstOrDefault();

            return cultureName != null 
                ? new CultureSelectorResult { Priority = 10, CultureName = cultureName }
                : null;
        }

        private static CultureInfo ParseCultureInfo(string cultureName)
        {
            try
            {
                return new CultureInfo(cultureName);
            }
            catch
            {
                return null;
            }
        }
    }
}