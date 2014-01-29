using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Orchard.Themes;
using SmartWalk.Server.Services;

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

        public ActionResult ImportXmlDataAction()
        {
            _importService.ImportXmlData();

            return new EmptyResult();
        } 
    }
}