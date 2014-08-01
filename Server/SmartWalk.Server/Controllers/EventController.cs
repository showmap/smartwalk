using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Web.Mvc;
using Orchard;
using Orchard.Themes;
using SmartWalk.Server.Controllers.Base;
using SmartWalk.Server.Extensions;
using SmartWalk.Server.Records;
using SmartWalk.Server.Services.EventService;
using SmartWalk.Server.Utils;
using SmartWalk.Server.ViewModels;
using SmartWalk.Server.Views;
using SmartWalk.Shared.Utils;

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
        public ActionResult List(DisplayType display = DisplayType.All, SortType sort = SortType.Date)
        {
            var parameters = new ListViewParametersVm { Display = display, Sort = sort };
            var result = GetEventVms(parameters);

            return View(new ListViewVm<EventMetadataVm>
                {
                    Parameters = parameters,
                    Data = result
                });
        }

        [CompressFilter]
        public ActionResult View(int eventId, int? day = null)
        {
            var eventDay = day != null ? Math.Max(day.Value - 1, 0) : 0;
            var result = _eventService.GetEventById(eventId, eventDay);
            if (result == null) return new HttpNotFoundResult();

            var view = View(result);
            view.ViewData.Add("day", eventDay + 1);
            return view;
        }

        [CompressFilter]
        public ActionResult Create()
        {
            if (CurrentSmartWalkUser == null) return new HttpUnauthorizedResult();

            return View(new EventMetadataVm());
        }

        [CompressFilter]
        public ActionResult Edit(int eventId)
        {
            if (CurrentSmartWalkUser == null) return new HttpUnauthorizedResult();

            var result = _eventService.GetEventById(eventId);
            if (result == null) return new HttpNotFoundResult();

            var access = _eventService.GetEventAccess(CurrentSmartWalkUser.Record, eventId);
            if (access != AccessType.AllowEdit) return new HttpUnauthorizedResult();

            return View(result);
        }

        [CompressFilter]
        public ActionResult DeleteEvent(int eventId)
        {
            if (CurrentSmartWalkUser == null) return new HttpUnauthorizedResult();

            _eventService.DeleteEvent(eventId);

            return RedirectToAction("List");
        }

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
            if (CurrentSmartWalkUser == null) return new HttpUnauthorizedResult();

            var errors = ValidateEvent(eventVm);
            if (errors.Count > 0)
            {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new ErrorResultVm(errors));
            }

            var result = _eventService.SaveEvent(CurrentSmartWalkUser.Record, eventVm);
            return Json(result);
        }

        private IList<EventMetadataVm> GetEventVms(
            ListViewParametersVm parameters, 
            int pageNumber = 0, 
            string query = null)
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

        // TODO: To validate if host is owned by current user
        // TODO: To validate if there are duplicated venues
        private IList<ValidationError> ValidateEvent(EventMetadataVm model)
        {
            var result = new List<ValidationError>();

            var startDateProperty = model.GetPropertyName(p => p.StartDate);
            if (!model.StartDate.HasValue)
            {
                result.Add(new ValidationError(
                               startDateProperty,
                               T("Start date can not be empty.").Text));
            }

            if (model.StartDate.HasValue && model.EndDate.HasValue &&
                model.StartDate.Value > model.EndDate.Value)
            {
                result.Add(new ValidationError(
                               startDateProperty,
                               T("Start date has to be less than or equal to the end date.").Text));
            }

            var pictureProperty = model.GetPropertyName(p => p.Picture);
            if (!string.IsNullOrEmpty(model.Picture))
            {
                if (model.Picture.Length > 255)
                {
                    result.Add(new ValidationError(
                                   pictureProperty,
                                   T("Picture URL can not be longer than 255 characters.").Text));
                }
                else if (!model.Picture.IsUrlValid())
                {
                    result.Add(new ValidationError(
                                   pictureProperty,
                                   T("Picture URL has bad format.").Text));
                }
            }

            if (model.Host == null)
            {
                result.Add(new ValidationError(
                               model.GetPropertyName(p => p.Host),
                               T("Event organizer can not be empty.").Text));
            }

            var venuesProperty = model.GetPropertyName(p => p.Venues);
            var showsProperty = Reflection<EntityVm>.GetProperty(p => p.Shows).Name;
            var venues = model.Venues != null 
                ? model.Venues.Where(v => !v.Destroy).ToArray() 
                : new EntityVm[] {};
            for (var i = 0; i < venues.Length; i++)
            {
                var venueVm = venues[i];
                var shows = venueVm.Shows != null
                    ? venueVm.Shows.Where(v => !v.Destroy).ToArray()
                    : new ShowVm[] { };
                for (var j = 0; j < shows.Length; j++)
                {
                    var showVm = shows[j];
                    result.AddRange(ValidateShow(
                        showVm,
                        model,
                        string.Format(
                            "{0}[{1}].{2}[{3}].", 
                            venuesProperty, 
                            i + 1,
                            showsProperty,
                            j + 1)));
                }
            }

            return result;
        }

        private IEnumerable<ValidationError> ValidateShow(ShowVm model, EventMetadataVm eventVm, string prefix = "")
        {
            var result = new List<ValidationError>();

            var titleProperty = model.GetPropertyName(p => p.Title);
            if (string.IsNullOrEmpty(model.Title))
            {
                result.Add(new ValidationError(
                               prefix + titleProperty,
                               T("Title can not be empty!").Text));
            }
            else if (model.Title.Length > 255)
            {
                result.Add(new ValidationError(
                               prefix + titleProperty,
                               T("Title can not be larger than 255 characters!").Text));
            }

            var pictureProperty = model.GetPropertyName(p => p.Picture);
            if (!string.IsNullOrEmpty(model.Picture))
            {
                if (model.Picture.Length > 255)
                {
                    result.Add(new ValidationError(
                                   prefix + pictureProperty,
                                   T("Picture url can not be larger than 255 characters!").Text));
                }
                else if (!model.Picture.IsUrlValid())
                {
                    result.Add(new ValidationError(
                                   prefix + pictureProperty,
                                   T("Picture url is in bad format!").Text));
                }
            }

            var detailsUrlProperty = model.GetPropertyName(p => p.DetailsUrl);
            if (!string.IsNullOrEmpty(model.DetailsUrl))
            {
                if (model.DetailsUrl.Length > 255)
                {
                    result.Add(new ValidationError(
                                   prefix + detailsUrlProperty,
                                   T("Details url can not be larger than 255 characters!").Text));
                }
                else if (!model.DetailsUrl.IsUrlValid())
                {
                    result.Add(new ValidationError(
                                   prefix + detailsUrlProperty,
                                   T("Details url is in bad format!").Text));
                }
            }

            var startTimeProperty = model.GetPropertyName(p => p.StartTime);
            if (model.StartTime.HasValue && model.EndTime.HasValue &&
                model.StartTime.Value > model.EndTime.Value)
            {
                result.Add(new ValidationError(
                               prefix + startTimeProperty,
                               T("Show start time has to be less than or equal to the end time.").Text));
            }

            if (model.StartTime.HasValue && eventVm.StartDate.HasValue &&
                (model.StartTime.Value < eventVm.StartDate.Value.AddDays(-1) ||
                (eventVm.EndDate.HasValue && model.StartTime.Value > eventVm.EndDate.Value.AddDays(1))))
            {
                result.Add(new ValidationError(
                               prefix + startTimeProperty,
                               T("Show start time has to be between event start and end dates.").Text));
            }

            if (model.EndTime.HasValue && eventVm.StartDate.HasValue &&
                (model.EndTime.Value < eventVm.StartDate.Value.AddDays(-1) ||
                (eventVm.EndDate.HasValue && model.EndTime.Value > eventVm.EndDate.Value.AddDays(2))))
            {
                result.Add(new ValidationError(
                               prefix + startTimeProperty,
                               T("Show end time has to be between event start and end dates.").Text));
            }

            return result;
        }
    }
}