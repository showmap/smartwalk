using System.Web.Mvc;
using SmartWalk.Server.Services;
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
            return Json(_queryService.ExecuteQuery(request));
        }
    }
}