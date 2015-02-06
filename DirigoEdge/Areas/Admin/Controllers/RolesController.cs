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
    public class RolesController : DirigoBaseAdminController
    {
        [PermissionsFilter(Permissions = "Can Edit Users")]
        public ActionResult ManageUserRoles()
        {
            var model = new ManageUserRolesViewModel();
            return View(model);
        }

        [PermissionsFilter(Permissions = "Can Edit Users")]
        public JsonResult AddUserRole(Role role)
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

            if (cfrp.RoleExists(role.RoleName))
            {
                // bail
                return result;
            }

            var NewRole = new Role
            {
                RoleId = Guid.NewGuid(),
                RoleName = role.RoleName,
                Permissions = role.Permissions
            };
            Context.Roles.Add(NewRole);
            success = Context.SaveChanges();

            // Add to WebSecurity as well
            Roles.CreateRole(role.RoleName);

            if (success > 0)
            {
                result.Data = new
                {
                    success = true,
                    message = "Role added successfully."
                };
            }
            return result;
        }

        [PermissionsFilter(Permissions = "Can Edit Users")]
        public JsonResult DeleteRole(Role role)
        {
            var result = new JsonResult()
            {
                Data = new { success = false, message = "There was an error processing your request." }
            };

            if (!String.IsNullOrEmpty(role.RoleId.ToString()))
            {
                // Delete from CodeFirst

                var RoleToDelete = Context.Roles.FirstOrDefault(x => x.RoleId == role.RoleId);

                if (RoleToDelete != null)
                {
                    Context.Roles.Remove(RoleToDelete);
                    var success = Context.SaveChanges();

                    if (success > 0)
                    {
                        result.Data = new { success = true, message = "The role has been successfully deleted." };
                    }

                    // Disallow deletion of administrators role
                    if (RoleToDelete.RoleName == "Administrators")
                    {
                        return result;
                    }
                }

                // Now Check WebSecurity
                if (!String.IsNullOrEmpty(role.RoleName) && role.RoleName != "Administrators")
                {
                    // Delete from WebSecurity
                    Roles.DeleteRole(role.RoleName);
                }
            }

            return result;
        }

        [PermissionsFilter(Permissions = "Can Edit Users")]
        public JsonResult ModifyRolePermissions(Role role)
        {
            var result = new JsonResult();

            if (!String.IsNullOrEmpty(role.RoleId.ToString()))
            {
                // Delete from CodeFirst
                var RoleToModify = Context.Roles.FirstOrDefault(x => x.RoleId == role.RoleId);

                // Don't modify Admin Permissions. They get everything
                if (RoleToModify.RoleName != "Administrators")
                {
                    // Now change permissions
                    RoleToModify.Permissions = role.Permissions;

                    Context.SaveChanges();
                }
            }

            return result;
        }

        [PermissionsFilter(Permissions = "Can Edit Users")]
        public JsonResult UpdateRoleCode(Role role)
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
            if (!String.IsNullOrEmpty(role.RoleId.ToString()))
            {
                // Delete from CodeFirst
                var RoleToModify = Context.Roles.FirstOrDefault(x => x.RoleId == role.RoleId);

                RoleToModify.RegistrationCode = role.RegistrationCode;

                success = Context.SaveChanges();
            }

            if (success > 0)
            {
                result.Data = new
                {
                    success = true,
                    message = "Registration code updated."
                };
            }

            return result;
        }


        public class RoleUsersModel
        {
            public Role Role { get; set;}
            public List<User>  Users { get; set; }
        }

        [PermissionsFilter(Permissions = "Can Edit Users")]
        public ActionResult GetRoleUsers(string RoleName)
        {
            var model = new RoleUsersModel
            {
                Role = Context.Roles.Where(x => x.RoleName == RoleName).ToList().FirstOrDefault(),
                Users = Context.Users.ToList()
            };

            return View("~/Areas/Admin/Views/Shared/Partials/GetRoleUsers.cshtml", model);
        }

        [PermissionsFilter(Permissions = "Can Edit Users")]
        public ActionResult ModifyUsersInRole(List<Guid> RemoveUsers, List<Guid> AddUsers, Guid RoleID)
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

            var role = Context.Roles.FirstOrDefault(x => x.RoleId == RoleID);

            if (role == null) { return result; }

            result.Data = new
            {
                success = true,
                message = "Users updated."
            };

            // Remove Users
            if (RemoveUsers != null && RemoveUsers.Any())
            {
                foreach (var gid in RemoveUsers)
                {
                    var user = Context.Users.Where(x => x.UserId == gid).FirstOrDefault();

                    if (user != null)
                    {
                        Roles.RemoveUserFromRole(user.Username, role.RoleName);
                        cfrp.RemoveUsersFromRoles(new string[] { user.Username }, new string[] { role.RoleName });
                    }
                }
            }

            // Add Users
            if (AddUsers != null && AddUsers.Any())
            {
                foreach (var gid in AddUsers)
                {
                    var user = Context.Users.Where(x => x.UserId == gid).FirstOrDefault();

                    if (user != null)
                    {
                        // Add to Membership Framework
                        if (!Roles.IsUserInRole(user.Username, role.RoleName))
                        {
                            Roles.AddUserToRole(user.Username, role.RoleName);
                        }

                        cfrp.AddUsersToRoles(new string[] { user.Username }, new string[] { role.RoleName });
                    }
                }
            }

            return result;
        }


    }

}