using Orchard;
using SmartWalk.Server.Controllers.Base;
using SmartWalk.Server.Records;
using SmartWalk.Server.Services.EntityService;
using SmartWalk.Server.Services.EventService;

namespace SmartWalk.Server.Controllers
{
    public class HostController : EntityBaseController
    {
        public HostController(
            IEntityService entityService,
            IEventService eventService,
            IOrchardServices orchardServices)
            : base(entityService, eventService, orchardServices)
        {
        }

        protected override EntityType EntityType
        {
            get { return EntityType.Host; }
        }
    }
}