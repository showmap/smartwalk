using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Configuration;
using System.Web.Mvc;
using Orchard.Logging;
using Orchard.Themes;
using Orchard.UI.Admin;
using SmartWalk.Server.Controllers.Base;
using SmartWalk.Server.Services.ImportService;
using SmartWalk.Server.ViewModels;
using SmartWalk.Shared;

namespace SmartWalk.Server.Controllers
{
    [Admin]
    public class AdminController : BaseController
    {
        private readonly IImportService _importService;

        public AdminController(IImportService importService)
        {
            _importService = importService;
        }

        [UsedImplicitly]
        public ILogger Logger { get; set; }

        public ActionResult Index()
        {
            var connection = WebConfigurationManager
                .AppSettings["Orchard.Azure.Media.StorageConnectionString"];
            var model = new AdminIndexVm
            {
                AzureStorageConnection = connection
            };
            return View(model);
        }

        public ActionResult ImportXmlData()
        {
            return View();
        }

        public ActionResult ImportImages()
        {
            return View();
        }

        public ActionResult ImportImagesResult(ImportItemType type)
        {
            var log = default(List<ImportImageResult>);

            try
            {
                log = _importService.ImportImages(type);
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, ex, "Error occured while importing images.");
                return View(new AdminImportImagesVm { Results = log, Error = ex.Message });
            }

            return View(new AdminImportImagesVm { Results = log });
        }

        [Themed]
        public ActionResult ImportXmlDataAction()
        {
            var log = default(List<string>);

            try
            {
                log = _importService.ImportXmlData();
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, ex, "Error occured while importing XML data");
                return new ContentResult { Content = GetContent(log, ex.Message) };
            }

            return new ContentResult { Content = GetContent(log) };
        }

        private static string GetContent(IEnumerable<string> log, string error = null)
        {
            if (log != null)
            {
                return log.Aggregate((i, j) => i + "<br/>" + j) +
                       (error != null ? ("<br/>Error: " + error) : "<br/>Import completed!");
            }

            return null;
        }
    }
}