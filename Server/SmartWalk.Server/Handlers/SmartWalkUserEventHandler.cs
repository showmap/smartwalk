using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement;
using Orchard.Data;
using Orchard.Mvc;
using Orchard.Roles.Models;
using Orchard.Roles.Services;
using Orchard.Security;
using Orchard.Users.Events;
using SmartWalk.Server.Extensions;
using SmartWalk.Server.Models;

namespace SmartWalk.Server.Handlers
{
    public class SmartWalkUserEventHandler : IUserEventHandler
    {
        private readonly IRoleService _roleService;
        private readonly IRepository<UserRolesPartRecord> _userRolesRepository;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IAuthorizer _authorizer;
        private readonly IContentManager _contentManager;

        public SmartWalkUserEventHandler(IHttpContextAccessor httpContext, IAuthorizer authorizer, IRoleService roleService, IRepository<UserRolesPartRecord> userRolesRepository, IContentManager contentManager)
        {
            _httpContext = httpContext;
            _authorizer = authorizer;
            _roleService = roleService;
            _userRolesRepository = userRolesRepository;
            _contentManager = contentManager;
        }

        public void LoggedOut(IUser user) {
            _httpContext.Current().Response.Redirect("~/");
        }

        public void Creating(UserContext context) {
            var swUser = context.User.As<SmartWalkUserPart>();

            if (swUser == null)
                return;

            swUser.CreatedAt = DateTime.UtcNow;
        }
        public void Created(UserContext context) {
            var role = _roleService.GetRoleByName(SmartWalkConstants.SmartWalkUserRole);
            if (role != null)
            {
                _userRolesRepository.Create(
                    new UserRolesPartRecord
                    {
                        UserId = context.User.As<IUser>().Id,
                        Role = role
                    });
            }
        }
        public void LoggedIn(IUser user)
        {
            var swUser = user.As<SmartWalkUserPart>();

            if (swUser == null)
                return;

            swUser.LastLoginAt = DateTime.UtcNow;
        }
        public void AccessDenied(IUser user) { }
        public void ChangedPassword(IUser user) { }
        public void SentChallengeEmail(IUser user) { }
        public void ConfirmedEmail(IUser user) { }
        public void Approved(IUser user) { }
    }
}