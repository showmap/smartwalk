using System.Web.Mvc;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Themes;
using SmartWalk.Server.Controllers.Base;
using SmartWalk.Server.Models;
using SmartWalk.Server.Records;
using SmartWalk.Server.Services.EntityService;
using SmartWalk.Server.Services.EventService;
using SmartWalk.Server.Views;

namespace SmartWalk.Server.Controllers
{
    [HandleError, Themed]
    public class HostController : EntityBaseController
    {
        private readonly IEntityService _entityService;
        private readonly IOrchardServices _orchardServices;

        public HostController(
            IOrchardServices orchardServices,
            IEntityService entityService,
            IEventService eventService)
            : base(
                orchardServices,
                entityService,
                eventService)
        {
            _orchardServices = orchardServices;
            _entityService = entityService;
        }

        protected override EntityType EntityType
        {
            get { return EntityType.Host; }
        }

        [HttpPost]
        public ActionResult AutoCompleteHost(string term)
        {
            if (_orchardServices.WorkContext.CurrentUser == null)
            {
                return new HttpUnauthorizedResult();
            }

            var user = _orchardServices.WorkContext.CurrentUser.As<SmartWalkUserPart>();
            return Json(
                _entityService.GetEntities(
                    user.Record, 
                    EntityType, 
                    0, 
                    ViewSettings.ItemsLoad, 
                    e => e.Name,
                    false,
                    term));
        }
    }
}