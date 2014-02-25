using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using SmartWalk.Server.Services;
using SmartWalk.Shared.DataContracts.Api;

namespace SmartWalk.Server.Controllers
{
    public class ApiController : Controller {
        private readonly IQueryService _queryService;

        public ApiController(IQueryService queryService) {
            _queryService = queryService;
        }

        public ActionResult Query() {
            Request.InputStream.Seek(0, SeekOrigin.Begin);
            var request = JsonConvert.DeserializeObject<Request>((new StreamReader(Request.InputStream).ReadToEnd()));
            return Json(_queryService.ExecuteQuery(request));
        }
    }
}