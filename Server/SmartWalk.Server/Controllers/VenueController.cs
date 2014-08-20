using Orchard;
using SmartWalk.Server.Controllers.Base;
using SmartWalk.Server.Records;
using SmartWalk.Server.Services.EntityService;

namespace SmartWalk.Server.Controllers
{
    public class VenueController : EntityBaseController
    {
        public VenueController(
            IEntityService entityService,
            IOrchardServices orchardServices)
            : base(entityService, orchardServices)
        {
        }

        protected override EntityType EntityType
        {
            get { return EntityType.Venue; }
        }
    }
}