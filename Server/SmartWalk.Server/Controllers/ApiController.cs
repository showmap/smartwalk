using System.Text;
using System.Web.Mvc;
using Newtonsoft.Json;
using SmartWalk.Server.Services.QueryService;
using SmartWalk.Shared.DataContracts.Api;

namespace SmartWalk.Server.Controllers
{
    public class ApiController : BaseController
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
    }
}