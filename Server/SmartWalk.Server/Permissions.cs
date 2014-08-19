using System.Collections.Generic;
using Orchard.Environment.Extensions.Models;
using Orchard.Security.Permissions;
using SmartWalk.Server.Utils;
using SmartWalk.Shared;

namespace SmartWalk.Server
{
    [UsedImplicitly]
    public class Permissions : IPermissionProvider
    {
        public static readonly Permission AccessFrontEnd = new Permission
            {
                Description = "Access SmartWalk front-end",
                Name = "AccessFrontEnd"
            };

        public static readonly Permission ViewAllContent = new Permission
            {
                Description = "View all SmartWalk content",
                Name = "ViewAllContent"
            };

        public static readonly Permission EditOwnContent = new Permission
            {
                Description = "Edit own SmartWalk content",
                Name = "EditOwnContent"
            };

        public static readonly Permission CreatePublicContent = new Permission
        {
            Description = "Create public SmartWalk content",
            Name = "CreatePublicContent"
        };

        public static readonly Permission UseAllContent = new Permission
            {
                Description = "Use all SmartWalk content",
                Name = "UseAllContent"
            };

        public static readonly Permission EditAllContent = new Permission
            {
                Description = "Edit all SmartWalk content",
                Name = "EditAllContent"
            };

        public virtual Feature Feature { get; set; }

        public IEnumerable<Permission> GetPermissions()
        {
            return new[] { AccessFrontEnd, ViewAllContent, EditOwnContent, UseAllContent, CreatePublicContent, EditAllContent };
        }

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes()
        {
            return new[]
                {
                    new PermissionStereotype
                        {
                            Name = "Administrator",
                            Permissions = new[] { EditAllContent, UseAllContent, CreatePublicContent, EditOwnContent, ViewAllContent, AccessFrontEnd }
                        },
                    new PermissionStereotype
                        {
                            Name = SmartWalkRoles.SmartWalkModerator,
                            Permissions = new[] { EditAllContent, UseAllContent, CreatePublicContent, EditOwnContent, ViewAllContent, AccessFrontEnd }
                        },
                    new PermissionStereotype
                        {
                            Name = SmartWalkRoles.SmartWalkProvisionEditor,
                            Permissions = new[] { CreatePublicContent, EditOwnContent, UseAllContent, ViewAllContent, AccessFrontEnd }
                        },
                    new PermissionStereotype
                        {
                            Name = SmartWalkRoles.SmartWalkVerifiedEditor,
                            Permissions = new[] { CreatePublicContent, EditOwnContent, ViewAllContent, AccessFrontEnd }
                        },
                    new PermissionStereotype
                        {
                            Name = SmartWalkRoles.SmartWalkEditor,
                            Permissions = new[] { EditOwnContent, ViewAllContent, AccessFrontEnd }
                        },
                    new PermissionStereotype
                        {
                            Name = SmartWalkRoles.SmartWalkViewer,
                            Permissions = new[] { ViewAllContent, AccessFrontEnd }
                        },
                    new PermissionStereotype
                        {
                            Name = "Anonymous",
                            Permissions = new[] { ViewAllContent, AccessFrontEnd }
                        }
                };
        }
    }
}