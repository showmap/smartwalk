using System.Collections.Generic;
using Orchard.Themes;
using SmartWalk.Server.Services;
using System;
using System.Web.Mvc;
using System.Linq;

namespace SmartWalk.Server.Controllers
{
    [HandleError, Themed]
    public class ToolsController : Controller {

        private readonly IImportService _importService;

        public ToolsController(IImportService importService) {
            _importService = importService;
        }

        public ActionResult ImportXmlData()
        {
            return View();
        }

        [Themed]
        public ActionResult ImportXmlDataAction()
        {
            var log = new List<string>();

            try
            {
                _importService.ImportXmlData(log);
            }
            catch (Exception ex)
            {
                return new ContentResult { Content = GetContent(log, ex.Message) };
            }

            return new ContentResult { Content = GetContent(log) };
        } 

        private static string GetContent(IEnumerable<string> log, string error = null)
        {
            return log.Aggregate((i, j) => i + "<br/>" + j) +
                   (error != null ? ("<br/>Error: " + error) : "Import completed!");
        }
    }
}