using System;
using Orchard.ContentManagement;
using Orchard.Data;
using Orchard.Mvc;
using Orchard.Roles.Models;
using Orchard.Roles.Services;
using Orchard.Security;
using Orchard.Users.Events;
using SmartWalk.Server.Models;
using SmartWalk.Server.Utils;
using SmartWalk.Shared;

namespace SmartWalk.Server.Handlers
{
    [UsedImplicitly]
    public class SmartWalkUserEventHandler : IUserEventHandler
    {
        private readonly IRoleService _roleService;
        private readonly IRepository<UserRolesPartRecord> _userRolesRepository;
        private readonly IHttpContextAccessor _httpContext;

        public SmartWalkUserEventHandler(
            IHttpContextAccessor httpContext,
            IRoleService roleService, 
            IRepository<UserRolesPartRecord> userRolesRepository)
        {
            _httpContext = httpContext;
            _roleService = roleService;
            _userRolesRepository = userRolesRepository;
        }

        public void LoggedOut(IUser user)
        {
            _httpContext.Current().Response.Redirect("~/");
        }

        public void Creating(UserContext context)
        {
            var swUser = context.User.As<SmartWalkUserPart>();
            if (swUser == null) return;

            if (swUser.CreatedAt == DateTime.MinValue)
            {
                swUser.CreatedAt = DateTime.UtcNow;
            }

            if (swUser.LastLoginAt == DateTime.MinValue)
            {
                swUser.LastLoginAt = DateTime.UtcNow;
            }
        }

        public void Created(UserContext context)
        {
            var viewerRole = _roleService.GetRoleByName(SmartWalkRoles.SmartWalkViewer);
            if (viewerRole != null)
            {
                _userRolesRepository.Create(
                    new UserRolesPartRecord
                        {
                            UserId = context.User.As<IUser>().Id,
                            Role = viewerRole
                        });
            }

            var editorRole = _roleService.GetRoleByName(SmartWalkRoles.SmartWalkEditor);
            if (editorRole != null)
            {
                _userRolesRepository.Create(
                    new UserRolesPartRecord
                        {
                            UserId = context.User.As<IUser>().Id,
                            Role = editorRole
                        });
            }
        }

        public void LoggedIn(IUser user)
        {
            var swUser = user.As<SmartWalkUserPart>();
            if (swUser == null) return;

            swUser.LastLoginAt = DateTime.UtcNow;
        }

        public void AccessDenied(IUser user)
        {
        }

        public void ChangedPassword(IUser user)
        {
        }

        public void SentChallengeEmail(IUser user)
        {
        }

        public void ConfirmedEmail(IUser user)
        {
        }

        public void Approved(IUser user)
        {
        }

        public void LoggingIn(string userNameOrEmail, string password)
        {
        }

        public void LogInFailed(string userNameOrEmail, string password)
        {
        }
    }
}