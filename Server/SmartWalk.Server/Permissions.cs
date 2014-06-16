using System.Collections.Generic;
using Orchard.Environment.Extensions.Models;
using Orchard.Security.Permissions;

namespace SmartWalk.Server
{
    public class Permissions : IPermissionProvider
    {
        public static readonly Permission SmartWalkAdmin = new Permission { Description = "Administrator for SmartWalk server", Name = "SmartWalk Admin" };        

        public virtual Feature Feature { get; set; }

        public IEnumerable<Permission> GetPermissions()
        {
            return new[] {
                SmartWalkAdmin
            };
        }

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes()
        {
            return new[] {
                new PermissionStereotype {
                    Name = "Administrator",
                    Permissions = new[] {SmartWalkAdmin}
                }
            };
        }
    }  
}