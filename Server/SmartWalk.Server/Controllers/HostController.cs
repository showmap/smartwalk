using System.Web.Mvc;
using Orchard;
using Orchard.Security;
using SmartWalk.Server.Controllers.Base;
using SmartWalk.Server.Extensions;
using SmartWalk.Server.Records;
using SmartWalk.Server.Services.EntityService;
using SmartWalk.Server.Utils;
using SmartWalk.Server.Views;
using SmartWalk.Shared.Utils;

namespace SmartWalk.Server.Controllers
{
    public class HostController : EntityBaseController
    {
        private readonly IEntityService _entityService;
        private readonly IAuthorizer _authorizer;

        public HostController(
            IEntityService entityService,
            IOrchardServices orchardServices)
            : base(entityService)
        {
            _entityService = entityService;
            _authorizer = orchardServices.Authorizer;
        }

        protected override EntityType EntityType
        {
            get { return EntityType.Host; }
        }

        [HttpPost]
        [CompressFilter]
        public ActionResult AutoCompleteHost(string term)
        {
            // allowing use all hosts for provision users
            var display =
                _authorizer.Authorize(Permissions.UseAllContent)
                    ? DisplayType.None.Include(DisplayType.All).Include(DisplayType.My)
                    : DisplayType.My;

            var result = _entityService.GetEntities(
                display, EntityType.Host, 0, 
                ViewSettings.AutocompleteItems,
                false, term);

            return Json(result);
        }
    }
}