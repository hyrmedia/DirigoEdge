using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using DirigoEdgeCore.Data.Entities;
using DirigoEdgeCore.Models;

namespace DirigoEdge.Areas.Admin.Models.ViewModels
{
    public class BookmarkViewModel : DirigoBaseModel
    {
        public List<Bookmark> Bookmarks;

        public BookmarkViewModel()
        {
            var membershipUser = Membership.GetUser();

            if (membershipUser == null) return;

            var username = membershipUser.UserName;
            var user = Context.Users.FirstOrDefault(x => x.Username == username);
            if (user != null)
            {
                Bookmarks = Context.Bookmarks.Where(x => x.UserId == user.UserId).OrderBy(x => x.Title).ToList();
            }
        }
    }
}