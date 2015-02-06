using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Security;
using DirigoEdgeCore.Data.Entities;
using DirigoEdgeCore.Models;

namespace DirigoEdge.Areas.Admin.Models.ViewModels
{
    public class ManageUsersViewModel : DirigoBaseModel
    {
        public List<User> Users;
        public List<string> RolesList;
        public readonly string NOUSERIMAGE = "/Areas/Admin/Content/Themes/Base/Images/User.png";

        public ManageUsersViewModel()
        {
            BookmarkTitle = "Manage Users";
            Users = Context.Users.OrderBy(x => x.Username).ToList();

            // Enumerate Roles in Memory so it doesn't get disposed prematurely
            foreach (var user in Users)
            {
                user.Roles = user.Roles;
            }

            // Make sure all users have a thumbnail of some sort
            foreach (var user in Users)
            {
                user.UserImageLocation = String.IsNullOrEmpty(user.UserImageLocation) ? NOUSERIMAGE : user.UserImageLocation;
            }

            RolesList = Roles.GetAllRoles().ToList();
        }
    }
}