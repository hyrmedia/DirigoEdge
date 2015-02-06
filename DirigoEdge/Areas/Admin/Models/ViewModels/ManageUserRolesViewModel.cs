using System.Collections.Generic;
using System.Linq;
using System.Web.Security;
using DirigoEdgeCore.Data.Entities;
using DirigoEdgeCore.Models;
using DirigoEdgeCore.Utils;

namespace DirigoEdge.Areas.Admin.Models.ViewModels
{
    public class ManageUserRolesViewModel : DirigoBaseModel
    {
        public Dictionary<Role, List<string>> RoleUsersKVP;
        public List<string> RolesList;

        public Dictionary<string, bool> RolePersmissionsList;

        public const string NOUSERIMAGE = "/Areas/Admin/Content/Themes/Base/Images/User.png";

        public ManageUserRolesViewModel()
        {
            BookmarkTitle = "Manage User Roles";
            RoleUsersKVP = new Dictionary<Role, List<string>>();

            RolesList = Roles.GetAllRoles().ToList();

            foreach (string role in RolesList)
            {
                Role theRole = Context.Roles.FirstOrDefault(x => x.RoleName == role);
                if (theRole != null)
                {
                    theRole.Permissions = theRole.Permissions;
                    RoleUsersKVP.Add(theRole, Roles.GetUsersInRole(role).ToList());
                }
            }

            RolePersmissionsList = UserRoleUtilities.GetRolePermissionsList(new RolePermissions());
        }
    }
}