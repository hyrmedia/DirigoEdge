using System;
using System.Collections.Generic;
using System.Linq;
using DirigoEdgeCore.Data.Entities;
using DirigoEdgeCore.Models;

namespace DirigoEdge.Areas.Admin.Models.ViewModels
{
    public class ManageBlogAuthorsViewModel : DirigoBaseModel
    {
        public List<BlogUser> Users;
        public const string NOUSERIMAGE = "/areas/admin/css/images/user.png";

        public ManageBlogAuthorsViewModel()
        {
            BookmarkTitle = "Manage Blog Authors";

            Users = Context.BlogUsers.OrderBy(x => x.DisplayName).ToList();

            // Make sure all users have a thumbnail of some sort
            foreach (var user in Users)
            {
                user.UserImageLocation = String.IsNullOrEmpty(user.UserImageLocation) ? NOUSERIMAGE : user.UserImageLocation;
            }
        }
    }
}