using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using DirigoEdgeCore.Controllers;
using DirigoEdgeCore.Membership;
using DirigoEdgeCore.Models;
using DirigoEdgeCore.Utils;
using DirigoEdgeCore.Models.ViewModels;
using DirigoEdgeCore.Data.Entities;
using DirigoEdge.CustomUtils;

namespace DirigoEdge.Controllers
{
    [Authorize]
    public class AccountController : DirigoBaseController
    {
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid && WebSecurity.Login(model.UserName, model.Password, model.RememberMe))
            {
                Context.LoginLog.Add(new LoginLog
                {
                    UserName = model.UserName,
                    IPAddress = WebUtils.ClientIPAddress(),
                    Date = DateTime.UtcNow
                });

                Context.SaveChanges();

                return RedirectToLocal(returnUrl);
            }

            var message = GetLoginError(model);

            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", message);
            return View(model);
        }

        private string GetLoginError(LoginModel model)
        {
            String message;
            var user = Context.Users.FirstOrDefault(usr => usr.Username == model.UserName);


            if (user != null && user.IsLockedOut)
            {
                message =
                    "Your account is locked from too many invalid attemptes.\r\n Please use 'Forgot Password' to reset it.";
            }
            else
            {
                message = "The user name or password provided is incorrect.";
            }
            return message;
        }

        public ActionResult LogOff()
        {
            WebSecurity.Logout();

            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                try
                {
                   UserRoleUtilities.RegisterUser(model, Context);

                    return RedirectToAction("Index", "Admin", new {area = "Admin"});
                }
                catch (MembershipCreateUserException e)
                {
                    ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
        

        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : "";
            //ViewBag.HasLocalPassword = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.HasLocalPassword = true;
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manage(LocalPasswordModel model)
        {
            ViewBag.HasLocalPassword = true;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            // ChangePassword will throw an exception rather than return false in certain failure scenarios.
            bool changePasswordSucceeded;
            try
            {
                changePasswordSucceeded = WebSecurity.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
            }
            catch (Exception)
            {
                changePasswordSucceeded = false;
            }

            if (changePasswordSucceeded)
            {
                return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
            }
                    
            ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordModel model)
        {
            var errorMessage = String.Empty;
            
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Try sending reset
            try
            {
                var question = WebSecurity.GetSecretQuestion(model.Email);
            }
            catch (MembershipPasswordException e)
            {
                errorMessage = e.Message;
            }

            if (String.IsNullOrEmpty(errorMessage))
            {
                var success = WebSecurity.SendResetPassword(Request.Url.Host, model.Email, model.Answer);

                if (success)
                {
                    return RedirectToAction("ResetPasswordSuccess");
                }

                ModelState.AddModelError("", "The answer did not match our records");

                return View(model);
            }
                
            ModelState.AddModelError("", errorMessage);
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult ResetPasswordSuccess()
        {
            var model = new ContentViewViewModel("resetpasswordsuccess");

            if (model.ThePage != null)
            {
                return View(model.TheTemplate.ViewLocation, model);
            }

            HttpContext.Response.StatusCode = 404;
            return View("~/Views/Home/Error404.cshtml");
        }


        [HttpPost]
        [AllowAnonymous]
        public JsonResult GetSecretQuestion(string email)
        {
            // check database to see if there has been a try already
            var errorMessage = String.Empty;
            var question = String.Empty;

            try
            {
                question = WebSecurity.GetSecretQuestion(email);
            }
            catch (MembershipPasswordException e)
            {
                errorMessage = e.Message;
            }

            var result = new JsonResult();
            result.Data = new { secret = question, error = !String.IsNullOrEmpty(errorMessage), errorMessage = errorMessage };

            return result;
        }

        [AllowAnonymous]
        public ActionResult ChangePassword(string key)
        {
            var model = new ChangePasswordModel(key);

            if (model.Key != null)
            {
                return View(model);
            }

            return RedirectToAction("Login", new { returnUrl = "/" });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = WebSecurity.GetUserFromKey(model.Key);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var u = Membership.GetUser(user.Username);
            if (u == null)
            {
                return RedirectToAction("Login");
            }

            WebSecurity.ChangePassword(u.UserName, model.Password);
            WebSecurity.ClearResetKey(u.UserName);

            return RedirectToAction("Login");
        }


        #region Helpers
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            
            return RedirectToAction("Index", "Home");
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
        }



        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

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
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    }
}
