using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Orchard;
using Orchard.Themes;
using SmartWalk.Server.Controllers.Base;
using SmartWalk.Server.Extensions;
using SmartWalk.Server.Records;
using SmartWalk.Server.Services.EntityService;
using SmartWalk.Server.Services.EventService;
using SmartWalk.Server.ViewModels;
using SmartWalk.Server.Views;

namespace SmartWalk.Server.Controllers
{
    [HandleError, Themed]
    public class EventController : OrchardBaseController
    {
        private readonly IEventService _eventService;

        public EventController(
            IEventService eventService,
            IOrchardServices orchardServices)
            : base(orchardServices)
        {
            _eventService = eventService;
        }

        [CompressFilter]
        public ActionResult List(
            DisplayType display = DisplayType.All, 
            SortType sort = SortType.Date)
        {
            var result = _eventService.GetEvents(
                CurrentSmartWalkUser == null || display == DisplayType.All
                    ? null
                    : CurrentSmartWalkUser.Record,
                0,
                ViewSettings.ItemsLoad,
                GetSortFunc(sort),
                sort == SortType.Date);

            return View(new ListViewVm<EventMetadataVm>
                {
                    Parameters = new ListViewParametersVm { Display = display, Sort = sort },
                    Data = result
                });
        }

        [CompressFilter]
        public ActionResult View(int eventId) {
            var eventVm = _eventService.GetEventVmById(eventId);

            if (eventVm.Id != eventId) return new HttpNotFoundResult();

            return View(eventVm);
        }

        [CompressFilter]
        public ActionResult Create()
        {
            return Edit(0);
        }

        [CompressFilter]
        public ActionResult Edit(int eventId)
        {
            if (CurrentSmartWalkUser == null) return new HttpUnauthorizedResult();

            var eventVm = _eventService.GetEventVmById(eventId);

            if (eventVm.Id != eventId) return new HttpNotFoundResult();

            var access = _eventService.GetEventAccess(CurrentSmartWalkUser.Record, eventId);

            if (access == AccessType.AllowEdit)
                return View(eventVm);

            return new HttpUnauthorizedResult();
        }

        [HttpPost]
        [CompressFilter]
        public ActionResult GetEvents(int pageNumber, string query, ListViewParametersVm parameters)
        {
            var result = _eventService.GetEvents(
                CurrentSmartWalkUser == null || parameters.Display == DisplayType.All
                    ? null
                    : CurrentSmartWalkUser.Record,
                pageNumber,
                ViewSettings.ItemsLoad,
                GetSortFunc(parameters.Sort),
                parameters.Sort == SortType.Date,
                query);
            return Json(result);
        }

        [HttpPost]
        [CompressFilter]
        public ActionResult SaveEvent(EventMetadataVm item)
        {
            if (CurrentSmartWalkUser == null) return new HttpUnauthorizedResult();

            var errors = ValidateEvent(item);
            return Json(errors.Count > 0 ? null : _eventService.SaveOrAddEvent(CurrentSmartWalkUser.Record, item));
        }

        [CompressFilter]
        public ActionResult DeleteEvent(int eventId)
        {
            if (CurrentSmartWalkUser == null) return new HttpUnauthorizedResult();

            _eventService.DeleteEvent(eventId);

            return RedirectToAction("List");
        }

        private static Func<EventMetadataRecord, IComparable> GetSortFunc(SortType sortType)
        {
            var result =
                sortType == SortType.Date
                    ? new Func<EventMetadataRecord, IComparable>(emr => emr.StartTime)
                    : (emr => emr.Title ?? emr.EntityRecord.Name);
            return result;
        }

        // TODO: To validate if host is owned by current user
        // TODO: To validate if there are duplicated venues
        private IDictionary<string, string> ValidateEvent(EventMetadataVm model)
        {
            var res = new Dictionary<string, string>();

            if (!model.StartDate.HasValue)
                res.Add("StartDate", T("StartDate can not be empty!").Text);

            if (!string.IsNullOrEmpty(model.Picture))
            {
                if (model.Picture.Length > 255)
                    res.Add("Picture", T("Picture url can not be larger than 255 characters!").Text);
                else if (!model.Picture.IsUrlValid())
                    res.Add("Picture", T("Picture url is in bad format!").Text);
            }

            if (model.Host == null)
                res.Add("Host", T("Event organizer can not be empty!").Text);

            return res;
        }

        // TODO: To validate show on event Save
        private IDictionary<string, string> ValidateShow(ShowVm model)
        {
            var res = new Dictionary<string, string>();

            if (string.IsNullOrEmpty(model.Title))
                res.Add("Title", T("Title can not be empty!").Text);
            else if (model.Title.Length > 255)
                res.Add("Title", T("Title can not be larger than 255 characters!").Text);

            if (!string.IsNullOrEmpty(model.Picture))
            {
                if (model.Picture.Length > 255)
                    res.Add("Picture", T("Picture url can not be larger than 255 characters!").Text);
                else if (!model.Picture.IsUrlValid())
                    res.Add("Picture", T("Picture url is in bad format!").Text);
            }

            if (!string.IsNullOrEmpty(model.DetailsUrl))
            {
                if (model.DetailsUrl.Length > 255)
                    res.Add("DetailsUrl", T("Details url can not be larger than 255 characters!").Text);
                else if (!model.DetailsUrl.IsUrlValid())
                    res.Add("DetailsUrl", T("Details url is in bad format!").Text);
            }

            return res;
        }
    }
}