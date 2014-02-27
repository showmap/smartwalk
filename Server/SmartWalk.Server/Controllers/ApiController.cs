using System.Text;
using System.Web.Mvc;
using Newtonsoft.Json;
using SmartWalk.Server.Controllers.Results;
using SmartWalk.Server.Services.QueryService;
using SmartWalk.Shared.DataContracts.Api;

namespace SmartWalk.Server.Controllers
{
    public class ApiController : Controller
    {
        private readonly IQueryService _queryService;

        public ApiController(IQueryService queryService)
        {
            _queryService = queryService;
        }

        public ActionResult Query(Request request)
        {
            var response = _queryService.ExecuteQuery(request);
            return Json(response);
        }

        // TODO: It doesn't work :-(
        protected override JsonResult Json(
            object data, 
            string contentType, 
            Encoding contentEncoding)
        {
            return new JsonNetResult
                {
                    Data = data,
                    ContentType = contentType,
                    ContentEncoding = contentEncoding,
                    JsonSerializerSettings = new JsonSerializerSettings
                        {
                            Formatting = Formatting.Indented,
                            NullValueHandling = NullValueHandling.Ignore
                        }
                };
        }
    }
}