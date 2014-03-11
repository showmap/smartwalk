using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Orchard;
using Orchard.Themes;
using Orchard.ContentManagement;
using SmartWalk.Server.Models;
using SmartWalk.Server.ViewModels;
using SmartWalk.Server.Services.HostService;

namespace SmartWalk.Server.Controllers
{
    [HandleError, Themed]
    public class HostController : BaseController
    {
        private readonly IOrchardServices _orchardServices;

        private readonly IHostService _hostService;

        public HostController(IOrchardServices orchardServices, IHostService hostService) {
            _orchardServices = orchardServices;

            _hostService = hostService;
        }

        public ActionResult List() {
            if (_orchardServices.WorkContext.CurrentUser == null)
            {
                return new HttpUnauthorizedResult();
            }

            var user = _orchardServices.WorkContext.CurrentUser.As<SmartWalkUserPart>();

            return View(_hostService.GetUserHosts(user.Record));
        }

        public ActionResult Edit(int hostId) {
            if (_orchardServices.WorkContext.CurrentUser == null)
            {
                return new HttpUnauthorizedResult();
            }

            var user = _orchardServices.WorkContext.CurrentUser.As<SmartWalkUserPart>();

            return View(_hostService.GetHostVmById(hostId));
        }

        public ActionResult Get(int hostId)
        {
            if (_orchardServices.WorkContext.CurrentUser == null)
            {
                return new HttpUnauthorizedResult();
            }

            var user = _orchardServices.WorkContext.CurrentUser.As<SmartWalkUserPart>();

            return View(_hostService.GetHostVmById(hostId));
        }

        [HttpPost]
        public ActionResult Add(EntityVm host)
        {
            if (_orchardServices.WorkContext.CurrentUser == null)
            {
                return new HttpUnauthorizedResult();
            }

            var user = _orchardServices.WorkContext.CurrentUser.As<SmartWalkUserPart>();

            try
            {
                _hostService.AddHost(user.Record, host);
            }
            catch
            {
                return Json(false);
            }

            return Json(true);
        }
    }
}