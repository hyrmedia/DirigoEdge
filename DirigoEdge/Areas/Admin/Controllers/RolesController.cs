using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using DirigoEdge.Areas.Admin.Models;
using DirigoEdge.Areas.Admin.Models.ViewModels;
using DirigoEdge.Attributes;
using DirigoEdge.Controllers.Base;
using DirigoEdgeCore.Data.Entities;
using DirigoEdgeCore.Membership;

namespace DirigoEdge.Areas.Admin.Controllers
{
    public class RolesController : WebBaseAdminController
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
                Permissions = new List<Permission>()
            };
            if (role.Permissions != null)
            {
                foreach (var permission in role.Permissions)
                {
                    var existingPermission = Context.Permissions.FirstOrDefault(x => x.PermissionId == permission.PermissionId);
                    if (existingPermission != null)
                    {
                        NewRole.Permissions.Add(existingPermission);
                    }
                }
            }
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

            if (String.IsNullOrEmpty(role.RoleId.ToString())) return result;

            var roleToDelete = Context.Roles.FirstOrDefault(x => x.RoleId == role.RoleId);
            
            if (roleToDelete == null) return result;
            
            var roleName = roleToDelete.RoleName;

            // Disallow deletion of administrators role
            if (roleName == "Administrators")
            {
                result.Data = new { success = false, message = "The administrator role cannot be deleted." };
                return result;
            }

            // Now Check WebSecurity
            if (!String.IsNullOrEmpty(roleName))
            {
                // Delete from WebSecurity
                try
                {
                    Roles.DeleteRole(roleName);
                    Context.Roles.Remove(roleToDelete);
                    var success = Context.SaveChanges();

                    if (success > 0)
                    {
                        result.Data = new { success = true, message = "The role has been successfully deleted." };
                    }
                }
                catch (Exception err)
                {
                    result.Data = new { success = false, message = err.Message };
                    return result;
                }
            }

            return result;
        }

        [PermissionsFilter(Permissions = "Can Edit Users")]
        public JsonResult ModifyRolePermissions(Role role)
        {
            var result = new JsonResult();

            if (!String.IsNullOrEmpty(role.RoleId.ToString()) && role.Permissions != null)
            {
                // Delete from CodeFirst
                var RoleToModify = Context.Roles.FirstOrDefault(x => x.RoleId == role.RoleId);

                // Don't modify Admin Permissions. They get everything
                if (RoleToModify.RoleName != "Administrators")
                {
                    // Now change permissions
                    var count = RoleToModify.Permissions.Count;
                    for (var i = count - 1; i >= 0; i--)
                    {
                        RoleToModify.Permissions.Remove(RoleToModify.Permissions.ElementAt(i));
                    }
                    foreach (var permission in role.Permissions)
                    {
                        var existingPermission = Context.Permissions.FirstOrDefault(x => x.PermissionId == permission.PermissionId);
                        if (existingPermission != null)
                        {
                            RoleToModify.Permissions.Add(existingPermission);
                        }
                    }

                    result.Data = Context.SaveChanges();
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

        public class UserForRoleComparison
        {
            public Guid UserId { get; set; }
            public String Username { get; set; }
        }
        public class RoleUsersModel
        {
            public List<UserForRoleComparison> UsersInRole { get; set; }
            public List<UserForRoleComparison> UsersNotInRole { get; set; }
        }

        [PermissionsFilter(Permissions = "Can Edit Users")]
        public ActionResult GetRoleUsers(string roleId)
        {
            // find the role
            var roleIdGuid = new Guid(roleId);
            var role = Context.Roles.FirstOrDefault(x => x.RoleId == roleIdGuid);

            // need role users
            var roleUsers = role.Users;

            // get the users in that role selected into new model
            var queryUsersInRole =
                roleUsers.Select(x => new UserForRoleComparison { UserId = x.UserId, Username = x.Username }).ToList();
            var usersInRole = new List<UserForRoleComparison>();
            usersInRole.AddRange(queryUsersInRole);


            // get users not in role selected into new model
            var tempUsers = roleUsers.Select(x => x.UserId).ToList();
            var queryUsersNotInRole = Context.Users
                .Where(x => !tempUsers.Contains(x.UserId))
                .Select(x => new UserForRoleComparison { UserId = x.UserId, Username = x.Username }).ToList();
            var usersNotInRole = new List<UserForRoleComparison>();
            usersNotInRole.AddRange(queryUsersNotInRole);


            var model = new RoleUsersModel
            {
                UsersInRole = usersInRole,
                UsersNotInRole = usersNotInRole
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