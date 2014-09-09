using System.Net;
using System.Web.Mvc;
using Orchard;
using Orchard.Security;
using Orchard.Themes;
using SmartWalk.Server.Extensions;
using SmartWalk.Server.Records;
using SmartWalk.Server.Services.EntityService;
using SmartWalk.Server.Utils;
using SmartWalk.Server.ViewModels;
using SmartWalk.Server.Views;
using SmartWalk.Shared.Utils;

namespace SmartWalk.Server.Controllers.Base
{
    [HandleError, Themed]
    public abstract class EntityBaseController : BaseController
    {
        private readonly IEntityService _entityService;
        private readonly EntityValidator _validator;
        private readonly IAuthorizer _authorizer;

        protected EntityBaseController(
            IEntityService entityService,
            IOrchardServices orchardServices)
        {
            _entityService = entityService;
            _authorizer = orchardServices.Authorizer;
            // ReSharper disable DoNotCallOverridableMethodsInConstructor
            _validator = new EntityValidator(_entityService, EntityType, T);
            // ReSharper restore DoNotCallOverridableMethodsInConstructor
        }

        protected abstract EntityType EntityType { get; }

        [CompressFilter]
        public ActionResult List(DisplayType display)
        {
            var access = _entityService.GetEntitiesAccess();
            if (access == AccessType.Deny) return new HttpUnauthorizedResult();

            var result = _entityService.GetEntities(
                display, EntityType, 0, 
                ViewSettings.ItemsPerScrollPage);

            var view = View(result);
            view.ViewData[ViewDataParams.ListParams] =
                new ListViewParametersVm { Display = display };
            view.ViewData[ViewDataParams.AllowedActions] =
                new AllowedActions
                    {
                        CanCreate = access == AccessType.AllowEdit
                    };
            return view;
        }

        [CompressFilter]
        public ActionResult View(int entityId)
        {
            var access = _entityService.GetEntityAccess(entityId);
            if (access == AccessType.Deny) return new HttpUnauthorizedResult();

            var result = _entityService.GetEntityById(entityId);
            if (result == null || result.Type != (int)EntityType) return new HttpNotFoundResult();

            var view = View(result);
            view.ViewData[ViewDataParams.AllowedActions] =
                new AllowedActions
                    {
                        CanEdit = access == AccessType.AllowEdit
                    };
            return view;
        }

        [Authorize]
        [CompressFilter]
        public ActionResult Create()
        {
            var access = _entityService.GetEntitiesAccess();
            if (access != AccessType.AllowEdit) return new HttpUnauthorizedResult();

            return View(new EntityVm { Type = (byte)EntityType});
        }

        [Authorize]
        [CompressFilter]
        public ActionResult Edit(int entityId)
        {
            var access = _entityService.GetEntityAccess(entityId);
            if (access != AccessType.AllowEdit) return new HttpUnauthorizedResult();

            var result = _entityService.GetEntityById(entityId);
            if (result == null || result.Type != (int)EntityType) return new HttpNotFoundResult();

            var view = View(result);
            view.ViewData[ViewDataParams.AllowedActions] =
                new AllowedActions
                    {
                        CanDelete =
                            access == AccessType.AllowEdit
                            && _entityService.IsDeletable(entityId)
                    };
            return view;
        }

        [Authorize]
        [CompressFilter]
        public ActionResult Delete(int entityId)
        {
            var access = _entityService.GetEntityAccess(entityId);
            if (access != AccessType.AllowEdit) return new HttpUnauthorizedResult();

            var result = _entityService.GetEntityById(entityId);
            if (result == null || result.Type != (int)EntityType) return new HttpNotFoundResult();

            _entityService.DeleteEntity(entityId);

            return RedirectToAction("List", new { display = DisplayType.My });
        }

        // TODO: To catch exceptions and return ErrorResultVm (with code) for all HttpPost methods

        [HttpPost]
        [CompressFilter]
        public ActionResult GetEntities(int pageNumber, string query, ListViewParametersVm parameters)
        {
            var result = _entityService.GetEntities(
                parameters.Display, EntityType, pageNumber,
                ViewSettings.ItemsPerScrollPage,
                false, query);

            return Json(result);
        }

        [HttpPost]
        [CompressFilter]
        public ActionResult AutoCompleteEntity(string term, bool onlyMine = true, int[] excludeIds = null)
        {
            var display =
                _authorizer.Authorize(Permissions.UseAllContent)
                    ? DisplayType.None.Include(DisplayType.All).Include(DisplayType.My)
                    : (onlyMine ? DisplayType.My : DisplayType.All);

            var result = _entityService.GetEntities(
                display, EntityType, 0, ViewSettings.AutocompleteItems, 
                false, term, excludeIds);

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