using System;
using System.Web.Mvc;
using System.Web.Security;
using Orchard;
using Orchard.Security;
using Orchard.Users.Events;
using Orchard.Users.Models;
using Orchard.Users.Services;
using Orchard.ContentManagement;
using Orchard.Mvc;
using Orchard.Utility.Extensions;
using Orchard.Mvc.Extensions;
using System.Text.RegularExpressions;
using SmartWalk.Server.Controllers.Base;
using SmartWalk.Server.Models;
using SmartWalk.Server.Services.SmartWalkUserService;
using SmartWalk.Server.ViewModels;
using Orchard.Themes;

namespace SmartWalk.Server.Controllers
{
    [Themed]
    public class AccountController : BaseController
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IMembershipService _membershipService;
        private readonly ISmartWalkUserService _swUserService;
        private readonly IUserService _userService;
        private readonly IOrchardServices _orchardServices;
        private readonly IUserEventHandler _userEventHandler;

        public AccountController(
            IAuthenticationService authenticationService,
            ISmartWalkUserService swUserService,
            IMembershipService membershipService,
            IUserService userService,
            IOrchardServices orchardServices,
            IUserEventHandler userEventHandler)
        {
            _authenticationService = authenticationService;
            _membershipService = membershipService;
            _swUserService = swUserService;
            _userService = userService;
            _orchardServices = orchardServices;
            _userEventHandler = userEventHandler;
        }

        private int MinPasswordLength
        {
            get { return _membershipService.GetSettings().MinRequiredPasswordLength; }
        }

        [HttpPost]
        [AlwaysAccessible]
        [ValidateInput(false)]
        public ActionResult Register(string userName, string email, string password, string confirmPassword,
            string returnUrl = null)
        {
            // ensure users can register
            var registrationSettings = _orchardServices.WorkContext.CurrentSite.As<RegistrationSettingsPart>();
            if (!registrationSettings.UsersCanRegister)
            {
                return HttpNotFound();
            }

            ViewData["PasswordLength"] = MinPasswordLength;

            if (ValidateRegistration(userName, email, password, confirmPassword))
            {
                // Attempt to register the user
                // No need to report this to IUserEventHandler because _membershipService does that for us
                var userParams = new CreateUserParams(userName, password, email, null, null, false);
                var userData = new SmartWalkUserVm
                    {
                        FirstName = userName,
                        LastName = userName,
                        CreatedAt = DateTime.UtcNow,
                        LastLoiginAt = DateTime.UtcNow
                    };
                var user = _swUserService.CreateUser(new SmartWalkUserParams(userParams, userData));

                _orchardServices.ContentManager.Create(user);

                if (user != null)
                {
                    if (user.As<UserPart>().EmailStatus == UserStatus.Pending)
                    {
                        var siteUrl = _orchardServices.WorkContext.CurrentSite.BaseUrl;
                        if (String.IsNullOrWhiteSpace(siteUrl))
                        {
                            siteUrl = HttpContext.Request.ToRootUrlString();
                        }

                        _userService.SendChallengeEmail(
                            user.As<UserPart>(),
                            nonce =>
                            Url.MakeAbsolute(
                                Url.Action(
                                    "ChallengeEmail",
                                    "Account",
                                    new { Area = "Orchard.Users", nonce }),
                                    siteUrl));

                        _userEventHandler.SentChallengeEmail(user);
                        return RedirectToAction(
                            "ChallengeEmailSent",
                            "Account",
                            new { Area = "Orchard.Users" });
                    }

                    if (user.As<UserPart>().RegistrationStatus == UserStatus.Pending)
                    {
                        return RedirectToAction(
                            "RegistrationPending",
                            "Account",
                            new { Area = "Orchard.Users" });
                    }

                    _authenticationService.SignIn(user, false /* createPersistentCookie */);

                    return this.RedirectLocal(returnUrl);
                }

                ModelState.AddModelError(
                    "_FORM",
                    T(ErrorCodeToString( /*createStatus*/MembershipCreateStatus.ProviderError)));
            }

            // If we got this far, something failed, redisplay form
            var shape = _orchardServices.New.Register();
            return new ShapeResult(this, shape);
        }

        [AlwaysAccessible]
        public ActionResult EditProfile()
        {
            if (_orchardServices.WorkContext.CurrentUser == null) return new HttpUnauthorizedResult();

            var user = _orchardServices.WorkContext.CurrentUser;

            return View(_swUserService.GetUserViewModel(user));
        }

        [HttpPost]
        [AlwaysAccessible]
        public ActionResult EditProfile(SmartWalkUserVm profile)
        {
            if (_orchardServices.WorkContext.CurrentUser == null) return new HttpUnauthorizedResult();

            return RedirectToAction("EditProfile");
        }

        private bool ValidateRegistration(string userName, string email, string password, string confirmPassword)
        {
            var validate = true;

            if (String.IsNullOrEmpty(userName))
            {
                ModelState.AddModelError("username", T("You must specify a username."));
                validate = false;
            }
            else
            {
                if (userName.Length >= 255)
                {
                    ModelState.AddModelError("username", T("The username you provided is too long."));
                    validate = false;
                }
            }

            if (String.IsNullOrEmpty(email))
            {
                ModelState.AddModelError("email", T("You must specify an email address."));
                validate = false;
            }
            else if (email.Length >= 255)
            {
                ModelState.AddModelError("email", T("The email address you provided is too long."));
                validate = false;
            }
            else if (!Regex.IsMatch(email, UserPart.EmailPattern, RegexOptions.IgnoreCase))
            {
                // http://haacked.com/archive/2007/08/21/i-knew-how-to-validate-an-email-address-until-i.aspx    
                ModelState.AddModelError("email", T("You must specify a valid email address."));
                validate = false;
            }

            if (!validate) return false;

            if (!_userService.VerifyUserUnicity(userName, email))
            {
                ModelState.AddModelError("userExists", T("User with that username and/or email already exists."));
            }

            if (password == null || password.Length < MinPasswordLength)
            {
                ModelState.AddModelError(
                    "password",
                    T("You must specify a password of {0} or more characters.", MinPasswordLength));
            }

            if (!String.Equals(password, confirmPassword, StringComparison.Ordinal))
            {
                ModelState.AddModelError("_FORM", T("The new password and confirmation password do not match."));
            }

            return ModelState.IsValid;
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://msdn.microsoft.com/en-us/library/system.web.security.membershipcreatestatus.aspx for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "Username already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A username for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return
                        "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return
                        "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return
                        "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
    }
}