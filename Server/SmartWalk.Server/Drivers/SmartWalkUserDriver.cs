using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.Localization;
using SmartWalk.Server.Models;

namespace SmartWalk.Server.Drivers
{
    public class SmartWalkUserDriver : ContentPartDriver<SmartWalkUserPart>
    {
        public Localizer T { get; set; }

        public SmartWalkUserDriver()
        {
            T = NullLocalizer.Instance;
        }

        protected override DriverResult Editor(SmartWalkUserPart part, dynamic shapeHelper)
        {
            return ContentShape("Parts_User_Edit", () => shapeHelper.EditorTemplate(TemplateName: "Parts/User", Model: part, Prefix: Prefix));
        }

        protected override DriverResult Editor(SmartWalkUserPart part, IUpdateModel updater, dynamic shapeHelper) {
            updater.TryUpdateModel(part, Prefix, null, null);            

            var user = part.User;
            updater.TryUpdateModel(user, Prefix, new[] { "Email" }, null);

            return Editor(part, shapeHelper);
        }
    }
}