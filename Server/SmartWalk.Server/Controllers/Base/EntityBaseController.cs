using System.Net;
using System.Web.Mvc;
using Orchard;
using SmartWalk.Server.Extensions;
using SmartWalk.Server.Records;
using SmartWalk.Server.Services.EntityService;
using SmartWalk.Server.Services.EventService;
using SmartWalk.Server.Utils;
using SmartWalk.Server.ViewModels;
using SmartWalk.Server.Views;

namespace SmartWalk.Server.Controllers.Base
{
    public abstract class EntityBaseController : OrchardBaseController
    {
        private readonly IEntityService _entityService;
        private readonly IEventService _eventService;
        private readonly EntityValidator _validator;

        protected EntityBaseController(
            IOrchardServices orchardServices,
            IEntityService entityService,
            IEventService eventService)
            : base(orchardServices)
        {
            _entityService = entityService;
            _eventService = eventService;
            _validator = new EntityValidator(_entityService, EntityTypeName, T);
        }

        protected abstract EntityType EntityType { get; }

        private string EntityTypeName
        {
            get { return EntityType == EntityType.Venue ? "Venue" : "Organizer"; }
        }

        [CompressFilter]
        public ActionResult List(DisplayType display)
        {
            var access = _entityService.GetEntitiesAccess();
            if (access == AccessType.Deny) return new HttpUnauthorizedResult();

            var result = _entityService.GetEntities(display, EntityType, 0, 
                ViewSettings.ItemsLoad, e => e.Name);

            var view = View(result);
            view.ViewData.Add(
                "sw.listParameters", 
                new ListViewParametersVm { Display = display });
            return view;
        }

        [CompressFilter]
        public ActionResult View(int entityId)
        {
            var access = _entityService.GetEntityAccess(entityId);
            if (access == AccessType.Deny) return new HttpUnauthorizedResult();

            var result = _entityService.GetEntityById(entityId);
            if (result == null || result.Type != (int)EntityType) return new HttpNotFoundResult();

            return View(result);
        }

        [CompressFilter]
        public ActionResult Create()
        {
            var access = _entityService.GetEntitiesAccess();
            if (access != AccessType.AllowEdit) return new HttpUnauthorizedResult();

            return View(new EntityVm { Type = (int)EntityType});
        }

        [CompressFilter]
        public ActionResult Edit(int entityId)
        {
            var access = _entityService.GetEntityAccess(entityId);
            if (access != AccessType.AllowEdit) return new HttpUnauthorizedResult();

            var result = _entityService.GetEntityById(entityId);
            if (result == null || result.Type != (int)EntityType) return new HttpNotFoundResult();

            return View(result);
        }

        [CompressFilter]
        public ActionResult Delete(int entityId)
        {
            var access = _entityService.GetEntityAccess(entityId);
            if (access != AccessType.AllowEdit) return new HttpUnauthorizedResult();

            var result = _entityService.GetEntityById(entityId);
            if (result == null || result.Type != (int)EntityType) return new HttpNotFoundResult();

            _entityService.DeleteEntity(entityId);

            return RedirectToAction("List");
        }

        // TODO: To catch exceptions and return ErrorResultVm (with code) for all HttpPost methods

        [HttpPost]
        [CompressFilter]
        public ActionResult GetEvents(int entityId)
        {
            var result = _eventService.GetEventsByEntity(entityId);
            return Json(result);
        }

        [HttpPost]
        [CompressFilter]
        public ActionResult GetEntities(int pageNumber, string query, ListViewParametersVm parameters)
        {
            var result = _entityService.GetEntities(
                parameters.Display,
                EntityType,
                pageNumber,
                ViewSettings.ItemsLoad,
                e => e.Name,
                false,
                query);

            return Json(result);
        }

        [HttpPost]
        [CompressFilter]
        public ActionResult AutoCompleteEntity(string term, bool onlyMine = true, int[] excludeIds = null)
        {
            var result = _entityService.GetEntities(
                onlyMine ? DisplayType.My : DisplayType.All,
                EntityType,
                0,
                ViewSettings.ItemsLoad,
                e => e.Name,
                false,
                term,
                excludeIds);

            return Json(result);
        }

        [HttpPost]
        [CompressFilter]
        public ActionResult Validate(string propName, EntityVm model)
        {
            var errors = _validator.ValidateEntity(model);
            if (errors.ContainsPropertyError(propName))
            {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new ErrorResultVm(errors));
            }

            return Json(true);
        }

        [HttpPost]
        [CompressFilter]
        public ActionResult SaveEntity(EntityVm entityVm)
        {
            var errors = _validator.ValidateEntity(entityVm);
            if (errors.Count > 0)
            {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new ErrorResultVm(errors));
            }

            var result = _entityService.SaveEntity(entityVm);
            return Json(result);
        }
    }
}