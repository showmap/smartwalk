using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;
using Orchard.Themes;
using SmartWalk.Server.Controllers.Base;
using SmartWalk.Server.Extensions;
using SmartWalk.Server.Records;
using SmartWalk.Server.Services.EventService;
using SmartWalk.Server.Utils;
using SmartWalk.Server.ViewModels;
using SmartWalk.Server.Views;

namespace SmartWalk.Server.Controllers
{
    [HandleError, Themed]
    public class EventController : BaseController
    {
        private readonly IEventService _eventService;
        private readonly EventValidator _validator;

        public EventController(IEventService eventService)
        {
            _eventService = eventService;
            _validator = new EventValidator(T);
        }

        [CompressFilter]
        public ActionResult List(
            DisplayType display = DisplayType.All,
            SortType sort = SortType.Date)
        {
            var access = _eventService.GetEventsAccess();
            if (access == AccessType.Deny) return new HttpUnauthorizedResult();

            var parameters = new ListViewParametersVm { Display = display, Sort = sort };
            var result = GetEventVms(parameters);

            var view = View(result);
            view.ViewData[ViewDataParams.ListParams] = parameters;
            view.ViewData[ViewDataParams.AllowedActions] =
                new AllowedActions
                    {
                        CanCreate = access == AccessType.AllowEdit
                    };
            return view;
        }

        [CompressFilter]
        public ActionResult View(int eventId, int? day = null)
        {
            var access = _eventService.GetEventAccess(eventId);
            if (access == AccessType.Deny) return new HttpUnauthorizedResult();

            var eventDay = day != null ? Math.Max(day.Value - 1, 0) : 0;
            var result = _eventService.GetEventById(eventId, eventDay);
            if (result == null) return new HttpNotFoundResult();

            var view = View(result);
            view.ViewData[ViewDataParams.Day] = day;
            view.ViewData[ViewDataParams.AllowedActions] =
                new AllowedActions
                    {
                        CanEdit = access == AccessType.AllowEdit
                    };
            return view;
        }

        [CompressFilter]
        public ActionResult Create()
        {
            var access = _eventService.GetEventsAccess();
            if (access != AccessType.AllowEdit) return new HttpUnauthorizedResult();

            return View(new EventMetadataVm());
        }

        [CompressFilter]
        public ActionResult Edit(int eventId, int? day = null)
        {
            var access = _eventService.GetEventAccess(eventId);
            if (access != AccessType.AllowEdit) return new HttpUnauthorizedResult();

            var result = _eventService.GetEventById(eventId);
            if (result == null) return new HttpNotFoundResult();

            var view = View(result);
            view.ViewData[ViewDataParams.Day] = day;
            view.ViewData[ViewDataParams.AllowedActions] =
                new AllowedActions
                    {
                        CanDelete = access == AccessType.AllowEdit
                    };
            return view;
        }

        [CompressFilter]
        public ActionResult DeleteEvent(int eventId)
        {
            var access = _eventService.GetEventAccess(eventId);
            if (access != AccessType.AllowEdit) return new HttpUnauthorizedResult();

            var result = _eventService.GetEventById(eventId);
            if (result == null) return new HttpNotFoundResult();

            _eventService.DeleteEvent(eventId);

            return RedirectToAction("List", new { display = DisplayType.My });
        }

        // TODO: To catch exceptions and return ErrorResultVm (with code) for all HttpPost methods

        [HttpPost]
        [CompressFilter]
        public ActionResult GetEvents(int pageNumber, string query, ListViewParametersVm parameters)
        {
            var result = GetEventVms(parameters, pageNumber, query);
            return Json(result);
        }

        [HttpPost]
        [CompressFilter]
        public ActionResult SaveEvent(EventMetadataVm eventVm)
        {
            var errors = _validator.ValidateEvent(eventVm);
            if (errors.Count > 0)
            {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new ErrorResultVm(errors));
            }

            var result = _eventService.SaveEvent(eventVm);
            return Json(result);
        }

        private IList<EventMetadataVm> GetEventVms(
            ListViewParametersVm parameters, 
            int pageNumber = 0, 
            string query = null)
        {
            var result = _eventService.GetEvents(
                parameters.Display,
                pageNumber,
                ViewSettings.ItemsLoad,
                GetSortFunc(parameters.Sort),
                parameters.Sort == SortType.Date,
                query);
            return result;
        }

        private static Func<EventMetadataRecord, IComparable> GetSortFunc(SortType sortType)
        {
            var result =
                sortType == SortType.Date
                    ? new Func<EventMetadataRecord, IComparable>(emr => emr.StartTime)
                    : (emr => emr.Title ?? emr.EntityRecord.Name);
            return result;
        }
    }
}