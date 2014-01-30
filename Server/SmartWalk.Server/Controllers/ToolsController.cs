using Orchard.Themes;
using SmartWalk.Server.Services;
using System;
using System.Web.Mvc;

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
            try
            {
                _importService.ImportXmlData();
            }
            catch (Exception ex)
            {
                return new ContentResult {Content = ex.Message};
            }

            return new EmptyResult();
        } 
    }
}