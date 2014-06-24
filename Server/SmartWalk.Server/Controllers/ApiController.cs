using System.Web.Mvc;
using SmartWalk.Server.Controllers.Base;
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
            var response = _queryService.ExecuteRequestQuery(request);
            return Json(response);
        }
    }
}