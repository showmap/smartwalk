using System;
using System.Globalization;
using Orchard;
using Orchard.ContentManagement;
using SmartWalk.Server.Models;
using SmartWalk.Server.Records;

namespace SmartWalk.Server.Services.Base
{
    public class OrchardBaseService
    {
        private readonly IOrchardServices _orchardServices;
        private StringComparer _nameComparer;

        protected OrchardBaseService(IOrchardServices orchardServices)
        {
            _orchardServices = orchardServices;
        }

        protected SmartWalkUserRecord CurrentUserRecord
        {
            get
            {
                var userPart = _orchardServices.WorkContext.CurrentUser.As<SmartWalkUserPart>();
                return userPart != null ? userPart.Record : null;
            }
        }

        protected SmartWalkUserPart CurrentUserPart
        {
            get
            {
                var userPart = _orchardServices.WorkContext.CurrentUser.As<SmartWalkUserPart>();
                return userPart;
            }
        }

        protected IOrchardServices Services
        {
            get { return _orchardServices; }
        }

        protected StringComparer NameComparer
        {
            get
            {
                if (_nameComparer == null)
                {
                    var ci = CultureInfo.GetCultureInfo(Services.WorkContext.CurrentCulture);
                    _nameComparer = StringComparer.Create(ci, false);
                }

                return _nameComparer;
            }
        }
    }
}