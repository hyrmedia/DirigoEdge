using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using DirigoEdge.Areas.Admin.Models;
using DirigoEdge.Areas.Admin.Models.ViewModels;
using DirigoEdge.Controllers;
using DirigoEdgeCore.Controllers;
using DirigoEdgeCore.Data.Entities;
using DirigoEdgeCore.Membership;

namespace DirigoEdge.Areas.Admin.Controllers
{
    public class UsersController : DirigoBaseAdminController
    {
        [PermissionsFilter(Permissions = "Can Edit Users")]
        public ActionResult ManageUsers()
        {
            var model = new ManageUsersViewModel();
            return View(model);
        }

        [PermissionsFilter(Permissions = "Can Edit Users")]
        public JsonResult ModifyUser(User user)
        {
            var result = new JsonResult()
            {
                Data = new
                {
                    success = false,
                    message = "There was an error processing your request."
                }
            };

            var success = 0;

            if (!String.IsNullOrEmpty(user.UserId.ToString()))
            {
                var cfrp = new CodeFirstRoleProvider(Context);
                var editUser = Context.Users.FirstOrDefault(x => x.UserId == user.UserId);

                editUser.Username = user.Username;
                editUser.FirstName = user.FirstName;
                editUser.LastName = user.LastName;
                editUser.Email = user.Email;
                editUser.UserImageLocation = user.UserImageLocation;
                editUser.IsLockedOut = user.IsLockedOut;

                if (user.Roles != null)
                {
                    // Modify the user roles
                    // First delete existing roles
                    foreach (var role in editUser.Roles)
                    {
                        // get current role for comparison
                        var rolesList = user.Roles.Select(a => a.RoleName).ToList();
                        var userRoles = Context.Roles.Where(x => rolesList.Contains(role.RoleName)).ToList();
                        var foundRole = !userRoles.Contains(role);

                        // Only remove roles if it's not in the new set
                        if (foundRole)
                        {
                            Roles.RemoveUserFromRole(user.Username, role.RoleName);
                            cfrp.RemoveUsersFromRoles(new string[] {user.Username}, new string[] {role.RoleName});
                        }
                    }


                    // Add the asigned roles
                    foreach (var role in user.Roles)
                    {
                        // Add to Membership Framework
                        if (!Roles.IsUserInRole(user.Username, role.RoleName))
                        {
                            Roles.AddUserToRole(user.Username, role.RoleName);
                        }

                        // Add to CodeFirst as well
                        cfrp.AddUsersToRoles(new string[] {user.Username}, new string[] {role.RoleName});
                    }

                    success = Context.SaveChanges();
                }
            }

            if (success > 0)
            {
                result.Data = new
                {
                    success = true,
                    message = "Changes saved successfully."
                };
            }
            return result;
        }

        [PermissionsFilter(Permissions = "Can Edit Users")]
        public JsonResult AddUser(User user)
        {
            var result = new JsonResult()
            {
                Data = new
                {
                    success = false,
                    message = "There was an error processing your request."
                }
            };
            var cfrp = new CodeFirstRoleProvider(Context);

            var success = 0;

            if (!String.IsNullOrEmpty(user.Username))
            {
                // Add to .Net Membership Framework First
                WebSecurity.CreateUserAndAccount(user.Username, user.Password, user.TimeZone);

                // Now add additional fields to  CodeFirst User
                var newlyAddedUser = Context.Users.FirstOrDefault(x => x.Username == user.Username);

                newlyAddedUser.CreateDate = DateTime.UtcNow;
                newlyAddedUser.FirstName = user.FirstName;
                newlyAddedUser.LastName = user.LastName;
                newlyAddedUser.Email = user.Email;
                newlyAddedUser.UserImageLocation = user.UserImageLocation;
                success = Context.SaveChanges();

                // Add the asigned roles
                if (user.Roles != null && user.Roles.Any())
                {
                    foreach (var role in user.Roles)
                    {
                        // Add to Membership Framework
                        Roles.AddUserToRole(user.Username, role.RoleName);

                        // Add to CodeFirst as well
                        cfrp.AddUsersToRoles(new string[] { user.Username }, new string[] { role.RoleName });
                    }
                }
            }

            if (success > 0)
            {
                result.Data = new
                {
                    success = true,
                    message = "User added successfully."
                };
            }
            return result;
        }


        [PermissionsFilter(Permissions = "Can Edit Users")]
        public JsonResult DeleteUser(User user)
        {
            var result = new JsonResult()
            {
                Data = new
                {
                    success = false,
                    message = "There was an error processing your request."
                }
            };

            var success = 0;

            if (!String.IsNullOrEmpty(user.UserId.ToString()))
            {
                var UserToDelete = Context.Users.FirstOrDefault(x => x.UserId == user.UserId);

                // Make sure user even exists
                if (UserToDelete == null)
                {
                    return result;
                }

                // Clean up Roles First
                foreach (var role in Roles.GetRolesForUser(UserToDelete.Username))
                {
                    Roles.RemoveUserFromRole(UserToDelete.Username, role);
                }

                // Clean Up CodeFirst Items
                var eventModule = Context.EventAdminModules.Where(x => x.User.UserId == UserToDelete.UserId);
                Context.EventAdminModules.RemoveRange(eventModule);

                var blogModule = Context.BlogAdminModules.Where(x => x.User.UserId == UserToDelete.UserId);
                Context.BlogAdminModules.RemoveRange(blogModule);

                success = Context.SaveChanges();

                // Finally Delete From Membership
                WebSecurity.DeleteUser(UserToDelete.Username);
            }

            if (success > 0)
            {
                result.Data = new
                {
                    success = true,
                    message = "User successfully deleted."
                };
            }

            return result;
        }

        [PermissionsFilter(Permissions = "Can Edit Users")]
        public JsonResult ChangeUserPassword(User user, string newPassword)
        {
            var result = new JsonResult()
            {
                Data = new
                {
                    success = false,
                    message = "There was an error processing your request."
                }
            };

            var success = 0;
            if (!String.IsNullOrEmpty(user.UserId.ToString()))
            {
                var userToUpdate = Context.Users.FirstOrDefault(x => x.UserId == user.UserId);
                var cfmp = new CodeFirstMembershipProvider();

                cfmp.ChangePassword(userToUpdate.Username, newPassword);

                success = Context.SaveChanges();
            }

            if (success > 0)
            {
                result.Data = new
                {
                    success = true,
                    message = "Password changed."
                };
            }

            return result;
        }

    }
}