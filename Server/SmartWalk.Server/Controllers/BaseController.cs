using System.Text;
using System.Web.Mvc;
using SmartWalk.Server.Extensions;

namespace SmartWalk.Server.Controllers
{
    public class BaseController : Controller
    {
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