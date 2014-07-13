using System.Web.Mvc;
using Orchard;
using Orchard.Themes;
using SmartWalk.Server.Controllers.Base;
using SmartWalk.Server.Records;
using SmartWalk.Server.Services.EntityService;
using SmartWalk.Server.Services.EventService;

namespace SmartWalk.Server.Controllers
{
    [HandleError, Themed]
    public class HostController : EntityBaseController
    {
        public HostController(
            IOrchardServices orchardServices,
            IEntityService entityService,
            IEventService eventService)
            : base(
                orchardServices,
                entityService,
                eventService)
        {
        }

        protected override EntityType EntityType
        {
            get { return EntityType.Host; }
        }
    }
}