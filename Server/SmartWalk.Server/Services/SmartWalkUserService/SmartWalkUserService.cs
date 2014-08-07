using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard;
using Orchard.Logging;
using Orchard.Security;
using Orchard.ContentManagement;
using SmartWalk.Server.Models;
using Orchard.Users.Models;
using Orchard.Users.Events;
using Orchard.DisplayManagement;
using SmartWalk.Shared;
using Orchard.Messaging.Services;
using Orchard.Localization;
using SmartWalk.Server.ViewModels;

namespace SmartWalk.Server.Services.SmartWalkUserService
{
    [UsedImplicitly]
    public class SmartWalkUserService : ISmartWalkUserService
    {
        private readonly IOrchardServices _orchardServices;
        private readonly IMembershipService _membershipService;
        private readonly IMessageService _messageService;
        private readonly IEnumerable<IUserEventHandler> _userEventHandlers;
        private readonly IShapeFactory _shapeFactory;
        private readonly IShapeDisplay _shapeDisplay;

        public SmartWalkUserService(
            IOrchardServices orchardServices,
            IMembershipService membershipService,
            IMessageService messageService,
            IEnumerable<IUserEventHandler> userEventHandlers,
            IShapeFactory shapeFactory,
            IShapeDisplay shapeDisplay
            ) {
            _orchardServices = orchardServices;
            _membershipService = membershipService;
            _messageService = messageService;
            _userEventHandlers = userEventHandlers;
            _shapeFactory = shapeFactory;
            _shapeDisplay = shapeDisplay;

            Logger = NullLogger.Instance;
            T = NullLocalizer.Instance;
            }

        public ILogger Logger { get; set; }
        public Localizer T { get; set; }

        public IUser CreateUser(SmartWalkUserParams smartWalkUserParams) {
            var createUserParams = smartWalkUserParams.UserParams;

            Logger.Information("CreateUser {0} {1}", createUserParams.Username, createUserParams.Email);

            var registrationSettings = _orchardServices.WorkContext.CurrentSite.As<RegistrationSettingsPart>();

            var user = _orchardServices.ContentManager.New<UserPart>("User");

            user.UserName = createUserParams.Username;
            user.Email = createUserParams.Email;
            user.NormalizedUserName = createUserParams.Username.ToLowerInvariant();
            user.HashAlgorithm = "SHA1";
            _membershipService.SetPassword(user, createUserParams.Password);

            if (registrationSettings != null)
            {
                user.RegistrationStatus = registrationSettings.UsersAreModerated ? UserStatus.Pending : UserStatus.Approved;
                user.EmailStatus = registrationSettings.UsersMustValidateEmail ? UserStatus.Pending : UserStatus.Approved;
            }

            if (createUserParams.IsApproved)
            {
                user.RegistrationStatus = UserStatus.Approved;
                user.EmailStatus = UserStatus.Approved;
            }

            var userContext = new UserContext { User = user, Cancel = false, UserParameters = createUserParams };
            foreach (var userEventHandler in _userEventHandlers)
            {
                userEventHandler.Creating(userContext);
            }

            if (userContext.Cancel)
            {
                return null;
            }

            var userData = smartWalkUserParams.UserData;
            var smartWalkUser = user.As<SmartWalkUserPart>();
            if (smartWalkUser != null)
            {
                smartWalkUser.FirstName = userData.FirstName;
                smartWalkUser.LastName = userData.LastName;
                smartWalkUser.CreatedAt = userData.CreatedAt;
                smartWalkUser.LastLoginAt = userData.LastLoiginAt;
            }

            _orchardServices.ContentManager.Create(user);

            foreach (var userEventHandler in _userEventHandlers)
            {
                userEventHandler.Created(userContext);
                if (user.RegistrationStatus == UserStatus.Approved)
                {
                    userEventHandler.Approved(user);
                }
            }

            if (registrationSettings != null
                && registrationSettings.UsersAreModerated
                && registrationSettings.NotifyModeration
                && !createUserParams.IsApproved)
            {
                var usernames = String.IsNullOrWhiteSpace(registrationSettings.NotificationsRecipients)
                                    ? new string[0]
                                    : registrationSettings.NotificationsRecipients.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var userName in usernames)
                {
                    if (String.IsNullOrWhiteSpace(userName))
                    {
                        continue;
                    }
                    var recipient = _membershipService.GetUser(userName);
                    if (recipient != null)
                    {
                        var template = _shapeFactory.Create("Template_User_Moderated", Arguments.From(createUserParams));
                        template.Metadata.Wrappers.Add("Template_User_Wrapper");

                        var parameters = new Dictionary<string, object> {
                            {"Subject", T("New account").Text},
                            {"Body", _shapeDisplay.Display(template)},
                            {"Recipients", new [] { recipient.Email }}
                        };

                        _messageService.Send("Email", parameters);
                    }
                }
            }

            return user;
        }


        public SmartWalkUserVm GetUserViewModel(IUser user)
        {
            var swUserPart = user.As<SmartWalkUserPart>();

            if(swUserPart == null)
                return new SmartWalkUserVm();

            return new SmartWalkUserVm {
                FirstName = swUserPart.FirstName,
                LastName = swUserPart.LastName,
                CreatedAt = swUserPart.CreatedAt,
                LastLoiginAt = swUserPart.LastLoginAt
            };
        }


        public void UpdateSmartWalkUser(SmartWalkUserVm profile, IUser user) {
            var swUserPart = user.As<SmartWalkUserPart>();

            if (swUserPart == null) return;

        }
    }
}