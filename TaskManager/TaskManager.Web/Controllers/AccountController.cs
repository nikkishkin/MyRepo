using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using BCrypt.Net;
using TaskManager.Core;
using TaskManager.Entities;
using TaskManager.Logic.Services;
using TaskManager.Web.Models;
using TaskManager.Web.Models.Account;

namespace TaskManager.Web.Controllers
{
    public class AccountController : TaskManagerController
    {
        public const string SessionKeyUserPrincipal = "SessionKeyUserPrincipal";

        public const string ControllerName = "Account";

        public const string LogInAction = "LogIn";
        public const string LogOutAction = "LogOut";
        public const string SignUpAction = "SignUp";

        private readonly TaskManagerBlo _manager;

        public AccountController()
        {
            _manager = new TaskManagerBlo(UnitOfWork);
        }

        public ActionResult LogIn(string returnUrl)
        {
            RestoreModelState();
            return View(new LogInModel {ReturnUrl = returnUrl});
        }

        public ActionResult LogOut()
        {
            UserPrincipal.CurrentPrincipal = UserPrincipal.Empty;
            FormsAuthentication.SignOut();
            return RedirectToAction(LogInAction);
        }

        // Verify that user exists and password is right
        private void Validate(User dbUser, LogInModel logInModel)
        {
            if (dbUser == null)
            {
                ModelState.AddModelError("Username", "This username doesn't exist");
            }
            else
            {
                try
                {
                    if (!BCrypt.Net.BCrypt.Verify(logInModel.Password, dbUser.Password))
                    {
                        ModelState.AddModelError("Password", "Password is wrong!");
                    }
                }
                catch (SaltParseException)
                {
                    ModelState.AddModelError("Password", "Password is wrong!");
                }
            }
        }

        [HttpPost]
        public ActionResult LogIn(LogInModel userModel)
        {
            User dbUser = null;
            if (ModelState.IsValid)
            {
                dbUser = _manager.GetUser(userModel.Username);;
                Validate(dbUser, userModel);
            }

            if (ModelState.IsValid)
            {
                Authorize(dbUser);

                var redirectResult = GetRedirectResultOfReturnUrl(userModel.ReturnUrl);
                if (redirectResult != null)
                {
                    return redirectResult;
                }

                if (UserPrincipal.CurrentPrincipal.IsManager)
                {
                    return Redirect(Url.Action(TeamsController.IndexAction, TeamsController.ControllerName));
                }
                return Redirect(Url.Action(TasksController.IndexAction, TasksController.ControllerName));
            }

            SaveModelState(ModelState);
            return RedirectToAction(LogInAction);
        }

        private void Authorize(User dbUser)
        {
            // Create principal
            UserPrincipal userPrincipal = new UserPrincipal(dbUser.Username, dbUser.Id, dbUser.TeamId, dbUser.IsManager ?? false);

            // Save it to this request and session
            string principalApplicationKey = Guid.NewGuid().ToString();
            UserPrincipal.CurrentPrincipal = userPrincipal;
            ControllerContext.HttpContext.User = userPrincipal;
            ControllerContext.HttpContext.Application[principalApplicationKey] = userPrincipal;

            // Set Forms auth cookie
            HttpCookie authCookie = GetAuthCookie(userPrincipal, principalApplicationKey);
            Response.Cookies.Add(authCookie);
        }

        private RedirectResult GetRedirectResultOfReturnUrl(string returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl))
            {
                Uri returnUri = new Uri(returnUrl, UriKind.RelativeOrAbsolute);
                if (!returnUri.IsAbsoluteUri)
                {
                    return Redirect(returnUrl);
                }
                Uri requestUrl = Request.Url;
                if (requestUrl != null)
                {
                    string requestHost = requestUrl.Host;
                    if (returnUri.Host == requestHost)
                    {
                        return Redirect(returnUrl);
                    }
                }
            }
            return null;
        }

        private HttpCookie GetAuthCookie(UserPrincipal userPrincipal, string principalApplicationKey)
        {
            HttpCookie initialCookie = FormsAuthentication.GetAuthCookie(userPrincipal.Identity.Name, false);
            FormsAuthenticationTicket initialTicket = FormsAuthentication.Decrypt(initialCookie.Value);
            FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(1, // version
                                                                                 userPrincipal.Identity.Name, // username
                                                                                 initialTicket.IssueDate, // issue date
                                                                                 initialTicket.Expiration, // expiration
                                                                                 false, // persistance
                                                                                 principalApplicationKey, // user data
                                                                                 FormsAuthentication.FormsCookiePath);
            // Build auth cookie
            return new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(authTicket));
        }

        public ActionResult SignUp(string returnUrl)
        {
            RestoreModelState();
            return View(new SignUpModel { ReturnUrl = returnUrl });
        }
        
        [HttpPost]
        public ActionResult SignUp(SignUpModel userModel)
        {
            if (ModelState.IsValid)
            {
                if (_manager.UserExists(userModel.Username))
                {
                    ModelState.AddModelError("Username", "Sorry! This username already exists");
                    SaveModelState(ModelState);
                    return RedirectToAction(SignUpAction);
                }

                User user = new User
                {
                    Email = userModel.Email,
                    First_Name = userModel.FirstName,
                    Last_Name = userModel.LastName,
                    Username = userModel.Username,
                    Password = BCrypt.Net.BCrypt.HashString(userModel.Password),
                    IsManager = userModel.IsManager
                };

                _manager.AddUser(user);

                Authorize(user);

                var redirectResult = GetRedirectResultOfReturnUrl(userModel.ReturnUrl);
                if (redirectResult != null)
                {
                    return redirectResult;
                }

                if (UserPrincipal.CurrentPrincipal.IsManager)
                {
                    return Redirect(Url.Action(TeamsController.IndexAction, TeamsController.ControllerName));
                }
                return Redirect(Url.Action(TasksController.IndexAction, TasksController.ControllerName));
            }

            SaveModelState(ModelState);
            return RedirectToAction(SignUpAction);
        }
    }
}
