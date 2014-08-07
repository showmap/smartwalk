using Orchard;
using Orchard.ContentManagement;
using SmartWalk.Server.Models;
using SmartWalk.Server.Records;

namespace SmartWalk.Server.Services.Base
{
    public class OrchardBaseService
    {
        private readonly IOrchardServices _orchardServices;

        protected OrchardBaseService(IOrchardServices orchardServices)
        {
            _orchardServices = orchardServices;
        }

        protected SmartWalkUserRecord CurrentUser
        {
            get
            {
                var userPart = _orchardServices.WorkContext.CurrentUser.As<SmartWalkUserPart>();
                return userPart != null ? userPart.Record : null;
            }
        }
    }
}