using System.Linq;
using System.Web.Mvc;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Themes;
using SmartWalk.Server.Controllers.Base;
using SmartWalk.Server.Models;
using SmartWalk.Server.Records;
using SmartWalk.Server.Services.EntityService;
using SmartWalk.Server.Services.EventService;
using SmartWalk.Server.ViewModels;
using SmartWalk.Server.Views;

namespace SmartWalk.Server.Controllers
{
    [HandleError, Themed]
    public class VenueController : EntityBaseController
    {
        private readonly IEntityService _entityService;
        private readonly IOrchardServices _orchardServices;

        public VenueController(
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
            get { return EntityType.Venue; }
        }

        [HttpPost]
        public ActionResult AutoCompleteVenue(string term, int eventId, EventMetadataVm currentEvent)
        {
            if (_orchardServices.WorkContext.CurrentUser == null)
            {
                return new HttpUnauthorizedResult();
            }

            var user = _orchardServices.WorkContext.CurrentUser.As<SmartWalkUserPart>();
            var result = _entityService.GetAccesibleUserVenues(
                user.Record, 
                eventId, 
                0,
                ViewSettings.ItemsLoad,
                term);

            if (eventId == 0 && currentEvent != null)
            {
                result =
                    result.Where(
                        e =>
                            currentEvent.AllVenues
                                .Where(v => v.State != VmItemState.Deleted && v.Id > 0)
                                .All(v => v.Id != e.Id))
                        .ToList();
            }

            return Json(result);
        }
    }
}