using System.Web.Mvc;
using Orchard.MediaProcessing.Services;
using SmartWalk.Server.Controllers.Base;
using SmartWalk.Server.Extensions;
using SmartWalk.Server.Records;
using SmartWalk.Server.Services.EntityService;
using SmartWalk.Server.Utils;
using SmartWalk.Server.Views;
using SmartWalk.Shared.Utils;

namespace SmartWalk.Server.Controllers
{
    public class VenueController : EntityBaseController
    {
        private readonly IEntityService _entityService;

        public VenueController(
            IEntityService entityService,
            IImageProfileManager imageProfileManager)
            : base(entityService, imageProfileManager)
        {
            _entityService = entityService;
        }

        protected override EntityType EntityType
        {
            get { return EntityType.Venue; }
        }

        [HttpPost]
        [CompressFilter]
        public ActionResult AutoCompleteVenue(string term, int[] excludeIds = null)
        {
            // using all public venues plus current user's private ones
            var display = DisplayType.None.Include(DisplayType.All).Include(DisplayType.My);

            var result = _entityService.GetEntities(
                display, EntityType.Venue, 0,
                ViewSettings.AutocompleteItems,
                false, term, excludeIds);

            return Json(result);
        }
    }
}