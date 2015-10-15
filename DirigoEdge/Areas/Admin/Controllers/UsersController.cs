using System;
using System.Web.Mvc;
using DirigoEdge.Areas.Admin.Models;
using DirigoEdge.Areas.Admin.Models.ViewModels;
using DirigoEdge.Controllers.Base;
using DirigoEdge.CustomUtils;
using DirigoEdgeCore.Data.Entities;

namespace DirigoEdge.Areas.Admin.Controllers
{
    public class UsersController : WebBaseAdminController
    {

        private WebUserUtils _webUserUtils { get; set; }
        protected WebUserUtils WebUserUtils
        {
            get { return _webUserUtils ?? (_webUserUtils = new WebUserUtils(Context)); }
        }


        [PermissionsFilter(Permissions = "Can Edit Users")]
        public ActionResult ManageUsers()
        {
            var model = new ManageUsersViewModel();
            return View(model);
        }


        [PermissionsFilter(Permissions = "Can Edit Users")]
        public JsonResult ModifyUser(User user)
        {
            try
            {
                WebUserUtils.UpdateUser(user);
                return new JsonResult
                {
                    Data = new
                    {
                        success = true,
                        message = "Changes saved successfully."
                    }
                };
            }
            catch (Exception ex)
            {
                return new JsonResult
                {
                    Data = new
                    {
                        success = false,
                        message = ex.Message
                    }
                };
            }
        }

        [PermissionsFilter(Permissions = "Can Edit Users")]
        public JsonResult AddUser(User user)
        {
            try
            {
                WebUserUtils.AddNewUser(user);

                return new JsonResult
                {
                    Data = new
                    {
                        success = true,
                        message = "User added successfully."
                    }
                };
            }
            catch (Exception ex)
            {
                return new JsonResult
                {
                    Data = new
                    {
                        success = false,
                        message = ex.Message
                    }
                };
            }
        }

        [PermissionsFilter(Permissions = "Can Edit Users")]
        public JsonResult DeleteUser(User user)
        {

            try
            {
                WebUserUtils.DeleteUser(user.UserId);
                return new JsonResult
                {
                    Data = new
                    {
                        success = true,
                        message = "User successfully deleted."
                    }
                };
            }
            catch (Exception ex)
            {
                return new JsonResult
                {
                    Data = new
                    {
                        success = false,
                        message = ex.Message
                    }
                };
            }
        }

        [PermissionsFilter(Permissions = "Can Edit Users")]
        public JsonResult ChangeUserPassword(User user, string newPassword)
        {
            try
            {
                WebUserUtils.ChangePassword(user.UserId, newPassword);

                return new JsonResult
                {
                    Data = new
                    {
                        success = true,
                        message = "Password changed."
                    }
                };
            }
            catch (Exception ex)
            {
                return new JsonResult
                {
                    Data = new
                    {
                        success = false,
                        message = "There was an error processing your request."
                    }
                };
            }
        }
    }
}