using Orchard;
using Orchard.ContentManagement;
using SmartWalk.Server.Models;

namespace SmartWalk.Server.Controllers.Base
{
    public abstract class OrchardBaseController : BaseController
    {
        private readonly IOrchardServices _orchardServices;

        protected OrchardBaseController(IOrchardServices orchardServices)
        {
            _orchardServices = orchardServices;
        }

        protected SmartWalkUserPart CurrentSmartWalkUser
        {
            get { return _orchardServices.WorkContext.CurrentUser.As<SmartWalkUserPart>(); }
        }
    }
}