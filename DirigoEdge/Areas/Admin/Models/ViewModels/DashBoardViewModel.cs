using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using DirigoEdgeCore.Data.Entities;
using DirigoEdgeCore.Models;

namespace DirigoEdge.Areas.Admin.Models.ViewModels
{
    public class DashBoardViewModel : DirigoBaseModel
    {
        public List<Blog> Blogs;
        public List<ContentModule> Modules;
        public List<ContentPage> Pages;
        public List<User> Users;
        public List<Blog> FeaturedBlogs;
        public List<Bookmark> Bookmarks;

        public DashBoardViewModel()
        {
            string username = Membership.GetUser().UserName;
            User user = Context.Users.FirstOrDefault(x => x.Username == username);
            if (user != null)
            {
                Blogs = Context.Blogs.OrderByDescending(x => x.Date).ThenBy(x => x.Title).Take(5).ToList();
                Modules =
                    Context.ContentModules.Where(module => module.ParentContentModuleId == null)
                        .OrderByDescending(x => x.CreateDate)
                        .Take(5)
                        .ToList();
                Pages =
                    Context.ContentPages.Where(page => page.ParentContentPageId == null)
                        .OrderByDescending(x => x.PublishDate)
                        .Take(5)
                        .ToList();
                Users = Context.Users.OrderByDescending(x => x.LastLoginDate).Take(5).ToList();
                FeaturedBlogs = Context.Blogs.Where(x => x.IsFeatured).OrderByDescending(x => x.Date).ThenBy(x => x.Title).Take(5).ToList();
                Bookmarks =
                    Context.Bookmarks.Where(x => x.UserId == user.UserId).OrderBy(x => x.Title).ToList();
            }
        }
    }
}