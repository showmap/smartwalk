using System.Text;
using System.Web.Mvc;
using Orchard.Localization;
using Orchard.Logging;
using SmartWalk.Server.Extensions;

namespace SmartWalk.Server.Controllers.Base
{
    public abstract class BaseController : Controller
    {
        protected BaseController()
        {
            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
        }

        protected Localizer T { get; private set; }
        protected ILogger Logger { get; set; }

        protected override JsonResult Json(
            object data, 
            string contentType, 
            Encoding contentEncoding, 
            JsonRequestBehavior behavior)
        {
            return new JsonNetResult
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding,
                JsonRequestBehavior = behavior
            };
        }
    }
}